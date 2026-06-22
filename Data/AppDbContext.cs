using Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class User : IdentityUser<int>, ISoftDeletable
{
    public string FullName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? SaId { get; set; }
    public string? Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? ProfileImage { get; set; }
    public int? CityId { get; set; }
    public int? UserTypeId { get; set; }
    public int? NationalityId { get; set; }
    public string? DeviceToken { get; set; }
    public int TokenVersion { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Email verification
    public string? VerifyToken { get; set; }
    public DateTime? VerifyTokenExpiry { get; set; }

    // Password reset
    public string? ResetToken { get; set; }
    public DateTime? ResetTokenExpiry { get; set; }

    public string? BankName { get; set; }
    public string? AccountNumber { get; set; }
    public string? Iban { get; set; }
    public string? SwiftCode { get; set; }
    public decimal DailyRate { get; set; }

    public City? City { get; set; }
    public UserType? UserType { get; set; }
    public Nationality? Nationality { get; set; }
    public ICollection<Attendance> Attendances { get; set; } = [];
    public ICollection<UserLocationAssignment> UserLocationAssignments { get; set; } = [];
    public ICollection<FinancialTransaction> FinancialTransactions { get; set; } = [];
}

public class Role : IdentityRole<int>;
public class UserRole : IdentityUserRole<int>;
public class UserClaim : IdentityUserClaim<int>;
public class UserLogin : IdentityUserLogin<int>;
public class RoleClaim : IdentityRoleClaim<int>;
public class UserToken : IdentityUserToken<int>;

public class City
{
    public int Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public ICollection<User> Users { get; set; } = [];
    public ICollection<Location> Locations { get; set; } = [];
}

public class UserType
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public ICollection<User> Users { get; set; } = [];
}

public class Nationality
{
    public int Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public ICollection<User> Users { get; set; } = [];
}

public class Location : ISoftDeletable
{
    public int Id { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.Column("SiteName")]
    public string Name { get; set; } = string.Empty;
    [System.ComponentModel.DataAnnotations.Schema.Column("SiteType")]
    public string? Type { get; set; }
    public string? License { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int? CityId { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Image { get; set; }
    public int? JoinedEmployee { get; set; }
    public int? ObserverCount { get; set; }
    public string? WorkHours { get; set; }
    public string? WorkType { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public City? City { get; set; }
    public ICollection<Attendance> Attendances { get; set; } = [];
    public ICollection<UserLocationAssignment> UserLocationAssignments { get; set; } = [];
    public ICollection<Shift> Shifts { get; set; } = [];
}

public class Attendance : ISoftDeletable
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int LocationId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public DateTime Date { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public User User { get; set; } = null!;
    public Location Location { get; set; } = null!;
}

public class Shift : ISoftDeletable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int? LocationId { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Location? Location { get; set; }
}

public class UserLocationAssignment : ISoftDeletable
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int LocationId { get; set; }
    public string? Role { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UnassignedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public User User { get; set; } = null!;
    public Location Location { get; set; } = null!;
}

public class FinancialTransaction : ISoftDeletable
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? PaymentTypeId { get; set; }
    public string? DeductionBasis { get; set; }
    public string? DeductionValue { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public User User { get; set; } = null!;
    public PaymentType? PaymentType { get; set; }
}

public class PaymentType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class OtpCode
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public int Attempts { get; set; }
    public bool IsUsed { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<User, Role, int, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>(options)
{
    public DbSet<City> Cities => Set<City>();
    public DbSet<UserType> UserTypes => Set<UserType>();
    public DbSet<Nationality> Nationalities => Set<Nationality>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<Shift> Shifts => Set<Shift>();
    public DbSet<UserLocationAssignment> UserLocationAssignments => Set<UserLocationAssignment>();
    public DbSet<FinancialTransaction> FinancialTransactions => Set<FinancialTransaction>();
    public DbSet<PaymentType> PaymentTypes => Set<PaymentType>();
    public DbSet<OtpCode> OtpCodes => Set<OtpCode>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Location>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Attendance>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Shift>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<UserLocationAssignment>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<FinancialTransaction>().HasQueryFilter(e => !e.IsDeleted);

        builder.Entity<User>(e =>
        {
            e.ToTable("Users");
            e.Property(u => u.FullName).HasMaxLength(200);
            e.Property(u => u.FirstName).HasMaxLength(100);
            e.Property(u => u.MiddleName).HasMaxLength(100);
            e.Property(u => u.LastName).HasMaxLength(100);
            e.Property(u => u.SaId).HasMaxLength(20);
            e.Property(u => u.DailyRate).HasColumnType("decimal(18,2)").HasDefaultValue(0);
            e.HasIndex(u => u.SaId).IsUnique().HasFilter("SaId IS NOT NULL");
            e.HasOne(u => u.City).WithMany(c => c.Users).HasForeignKey(u => u.CityId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(u => u.UserType).WithMany(ut => ut.Users).HasForeignKey(u => u.UserTypeId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(u => u.Nationality).WithMany(n => n.Users).HasForeignKey(u => u.NationalityId).OnDelete(DeleteBehavior.SetNull);
            e.Property(u => u.VerifyToken).HasMaxLength(500);
            e.Property(u => u.ResetToken).HasMaxLength(500);
            e.HasIndex(u => u.VerifyToken).IsUnique().HasFilter("VerifyToken IS NOT NULL");
            e.HasIndex(u => u.ResetToken).IsUnique().HasFilter("ResetToken IS NOT NULL");
        });

        builder.Entity<Role>().ToTable("Roles");
        builder.Entity<UserRole>().ToTable("UserRoles");
        builder.Entity<UserClaim>().ToTable("UserClaims");
        builder.Entity<UserLogin>().ToTable("UserLogins");
        builder.Entity<RoleClaim>().ToTable("RoleClaims");
        builder.Entity<UserToken>().ToTable("UserTokens");

        builder.Entity<City>(e =>
        {
            e.ToTable("Cities");
            e.Property(c => c.NameAr).HasMaxLength(100);
            e.Property(c => c.NameEn).HasMaxLength(100);
        });

        builder.Entity<UserType>(e =>
        {
            e.ToTable("UserTypes");
            e.Property(ut => ut.Code).HasMaxLength(50);
            e.Property(ut => ut.NameAr).HasMaxLength(100);
            e.Property(ut => ut.NameEn).HasMaxLength(100);
            e.Property(ut => ut.Key).HasMaxLength(50);
        });

        builder.Entity<Nationality>(e =>
        {
            e.ToTable("Nationalities");
            e.Property(n => n.NameAr).HasMaxLength(100);
            e.Property(n => n.NameEn).HasMaxLength(100);
            e.Property(n => n.Code).HasMaxLength(10);
        });

        builder.Entity<Location>(e =>
        {
            e.ToTable("Locations");
            e.Property(l => l.Name).HasMaxLength(200);
            e.Property(l => l.Type).HasMaxLength(100);
            e.Property(l => l.License).HasMaxLength(100);
            e.Property(l => l.Phone).HasMaxLength(20);
            e.HasOne(l => l.City).WithMany(c => c.Locations).HasForeignKey(l => l.CityId).OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<Attendance>(e =>
        {
            e.ToTable("Attendances");
            e.Property(a => a.Status).HasMaxLength(20);
            e.HasOne(a => a.User).WithMany(u => u.Attendances).HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(a => a.Location).WithMany(l => l.Attendances).HasForeignKey(a => a.LocationId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(a => new { a.UserId, a.Date });
            e.HasIndex(a => new { a.Date, a.Status });
            e.HasIndex(a => new { a.LocationId, a.Date });
        });

        builder.Entity<Shift>(e =>
        {
            e.ToTable("Shifts");
            e.Property(s => s.Name).HasMaxLength(200);
            e.HasOne(s => s.Location).WithMany(l => l.Shifts).HasForeignKey(s => s.LocationId).OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<UserLocationAssignment>(e =>
        {
            e.ToTable("UserLocationAssignments");
            e.Property(ula => ula.Role).HasMaxLength(50);
            e.HasOne(ula => ula.User).WithMany(u => u.UserLocationAssignments).HasForeignKey(ula => ula.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(ula => ula.Location).WithMany(l => l.UserLocationAssignments).HasForeignKey(ula => ula.LocationId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(ula => new { ula.UserId, ula.LocationId }).IsUnique().HasFilter("IsDeleted = 0");
        });

        builder.Entity<FinancialTransaction>(e =>
        {
            e.ToTable("FinancialTransactions");
            e.Property(ft => ft.Type).HasMaxLength(50);
            e.Property(ft => ft.DeductionBasis).HasMaxLength(50);
            e.Property(ft => ft.DeductionValue).HasMaxLength(100);
            e.Property(ft => ft.Amount).HasColumnType("decimal(18,2)");
            e.HasOne(ft => ft.User).WithMany(u => u.FinancialTransactions).HasForeignKey(ft => ft.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(ft => ft.PaymentType).WithMany().HasForeignKey(ft => ft.PaymentTypeId).OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(ft => new { ft.UserId, ft.TransactionDate });
        });

        builder.Entity<PaymentType>(e =>
        {
            e.ToTable("PaymentTypes");
            e.Property(pt => pt.Name).HasMaxLength(100);
        });

        builder.Entity<OtpCode>(e =>
        {
            e.ToTable("OtpCodes");
            e.Property(o => o.Email).HasMaxLength(200);
            e.Property(o => o.Code).HasMaxLength(10);
            e.Property(o => o.Type).HasMaxLength(50);
            e.HasIndex(o => new { o.Email, o.Type });
        });
    }
}
