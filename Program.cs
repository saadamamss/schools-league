using Auth;
using Attendance;
using Common;
using Config;
using Data;
using Data.Queries;
using FinancialTransactions;
using Locations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using Services;
using Shifts;
using System.Reflection;
using System.Text;
using Upload;
using Users;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .WriteTo.Console()
    .WriteTo.File("logs/api-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14)
    .CreateLogger();

builder.Host.UseSerilog();

// JWT
var jwtSection = builder.Configuration.GetSection("JwtSettings");
var jwtKey = jwtSection["Key"]!;
if (string.IsNullOrEmpty(jwtKey) || jwtKey.Length < 32)
    throw new InvalidOperationException("JWT secret key must be at least 32 characters long. Set via JwtSettings__Key env var.");

builder.Services.Configure<JwtSettings>(jwtSection);
builder.Services.Configure<SeedSettings>(builder.Configuration.GetSection("SeedSettings"));
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.Configure<TimeZoneSettings>(builder.Configuration.GetSection("TimeZoneSettings"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSection["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };

        // Validate token_version claim — ensures logout invalidates existing JWTs
        options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var principal = context.Principal;
                if (principal?.FindFirst("token_version")?.Value is not string versionStr
                    || !int.TryParse(versionStr, out var tokenVersion))
                {
                    context.Fail("Token version claim missing.");
                    return;
                }

                var userIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    context.Fail("User ID claim missing.");
                    return;
                }

                var userManager = context.HttpContext.RequestServices
                    .GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<Data.User>>();
                var user = await userManager.FindByIdAsync(userIdClaim);
                if (user == null || user.IsDeleted || !user.IsActive || user.TokenVersion != tokenVersion)
                {
                    context.Fail("Token is no longer valid. Please log in again.");
                }
            }
        };
    });

builder.Services.AddAuthorization();

// DB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Identity
builder.Services.AddIdentity<User, Role>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// AddIdentity overrides auth schemes with cookies. Keep JWT as default for API.
builder.Services.Configure<Microsoft.AspNetCore.Authentication.AuthenticationOptions>(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
});

// CORS
var corsOrigins = builder.Configuration.GetSection("CorsSettings:Origins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        if (corsOrigins.Length == 1 && corsOrigins[0] == "*")
        {
            // Wildcard — AllowAnyOrigin, but do NOT call AllowCredentials (incompatible per spec)
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        }
        else if (corsOrigins.Length != 0)
        {
            policy.WithOrigins(corsOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
        }
        else
        {
            // No origins configured — safe default
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        }
    });
});

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("global", o =>
    {
        o.PermitLimit = 100;
        o.Window = TimeSpan.FromMinutes(1);
        o.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        o.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter("login", o =>
    {
        o.PermitLimit = 5;
        o.Window = TimeSpan.FromMinutes(1);
        o.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        o.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter("register", o =>
    {
        o.PermitLimit = 3;
        o.Window = TimeSpan.FromMinutes(1);
        o.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        o.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter("password_reset", o =>
    {
        o.PermitLimit = 3;
        o.Window = TimeSpan.FromMinutes(1);
        o.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        o.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter("otp_verify", o =>
    {
        o.PermitLimit = 5;
        o.Window = TimeSpan.FromMinutes(1);
        o.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        o.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter("upload", o =>
    {
        o.PermitLimit = 10;
        o.Window = TimeSpan.FromMinutes(1);
        o.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        o.QueueLimit = 0;
    });

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            data = (object?)null,
            status = new { message = "Too many requests. Please try again later.", code = 429, success = false }
        }, token);
    };
});

// Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<LocationsService>();
builder.Services.AddScoped<UserLocationAssignmentsService>();
builder.Services.AddScoped<AttendanceService>();
builder.Services.AddScoped<ShiftsService>();
builder.Services.AddScoped<FinancialTransactionsService>();
builder.Services.AddScoped<UploadService>();
builder.Services.AddScoped<AttendanceStatisticsQuery>();
builder.Services.AddScoped<UserAttendanceQueries>();
builder.Services.AddScoped<LocationStatisticsQueries>();
builder.Services.AddScoped<IMailService, MailService>();

// Controllers + JSON
builder.Services.AddControllers(options =>
{
    // Exception handling is done globally via ExceptionMiddleware in the pipeline
}).AddJsonOptions(o =>
{
    o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    o.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    o.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Schools League API",
        Version = "v1",
        Description = "API for managing schools league attendance, users, locations and shifts."
    });
    options.EnableAnnotations();
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter JWT token"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// FluentValidation (TODO: add validators when needed)
// builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// Health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("database");

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

// Forwarded headers — needed behind Railway's TLS proxy (railway-hikari)
// so the app sees the original HTTPS scheme, client IP, etc.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor;
    // Only trust the known proxy network (Railway internal)
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

// Middleware pipeline — ORDER MATTERS
// 1. Compression first (before security headers to compress them)
app.UseResponseCompression();

// 2. Security headers — MUST be near the start so they're set even
//    on short-circuited responses (CORS preflight, rate-limited, etc.)
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    context.Response.Headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";
    context.Response.Headers["Content-Security-Policy"] = "default-src 'self'; img-src 'self' data:; font-src 'self'; style-src 'self' 'unsafe-inline'";
    if (context.Request.IsHttps)
        context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
    await next();
});

// 3. Forwarded headers (before security redirect, so HttpsRedirection sees the real scheme)
app.UseForwardedHeaders();

// 4. Exception handler wraps all downstream middleware
app.UseMiddleware<ExceptionMiddleware>();

// 5. Request logging (inside exception handler's try/catch)
app.UseMiddleware<Common.Middleware.LoggingMiddleware>();
app.UseSerilogRequestLogging();

// 6. Security redirect
app.UseHttpsRedirection();

// 7. CORS (must be before auth)
app.UseCors();

// 8. Rate limiter
app.UseRateLimiter();

// 9. Cookie-to-header bridge (before auth)
app.UseMiddleware<Common.Middleware.CookieToHeaderMiddleware>();

// 10. Auth
app.UseAuthentication();
app.UseAuthorization();

// Swagger (dev only)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Static files for uploads
app.UseStaticFiles();

// Health check endpoints
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (ctx, report) =>
    {
        ctx.Response.ContentType = "application/json";
        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new { name = e.Key, status = e.Value.Status.ToString() }),
            timestamp = DateTime.UtcNow
        });
        await ctx.Response.WriteAsync(result);
    }
});
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => true
});

// Controllers
app.MapControllers();

// Migration + Seed (all environments; seed is idempotent)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    if (!app.Environment.IsDevelopment())
        Log.Information("Database migrated successfully in {Env} environment.", app.Environment.EnvironmentName);
    await DbSeedr.SeedAsync(app.Services);
}

// Configuration validation (production only)
if (!app.Environment.IsDevelopment())
{
    var jwtKeyCheck = app.Configuration["JwtSettings:Key"];
    if (string.IsNullOrEmpty(jwtKeyCheck) || jwtKeyCheck == "CHANGE-ME-32-CHAR-SECRET-KEY-HERE!" || jwtKeyCheck.Length < 32)
        Log.Error("JWT secret key is not properly set for production. Set via JwtSettings__Key env var.");

    var mailHost = app.Configuration["MailSettings:Host"];
    if (string.IsNullOrEmpty(mailHost))
        Log.Warning("Mail server not configured. Email features (registration, password reset) will fail. Set via MailSettings__Host env var.");

    var connStr = app.Configuration.GetConnectionString("DefaultConnection");
    if (connStr?.Contains("root") == true || connStr?.Contains("pass1234") == true)
        Log.Warning("Database connection string appears to use default dev credentials. Override via ConnectionStrings__DefaultConnection env var.");
}

// Graceful shutdown
app.Lifetime.ApplicationStopping.Register(() =>
{
    Log.Information("Application shutting down...");
    Log.CloseAndFlush();
});

app.Run();
