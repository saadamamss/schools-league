# Schools League - Backend

REST API for attendance and workforce management. Built with ASP.NET Core 8.

## Tech Stack

- **ASP.NET Core 8** (Web API)
- **Entity Framework Core** (ORM)
- **MySQL 8.0** (database)
- **ASP.NET Identity** (authentication)
- **JWT Bearer** (token auth)
- **MailKit** (email service)
- **ClosedXML** (Excel export)
- **Serilog** (logging)
- **Swagger/OpenAPI** (API docs)

## Project Structure

```
Auth/                 # Authentication (login, register, refresh, password reset)
Users/                # User management (CRUD, export)
Attendance/           # Attendance tracking (check-in/out, records)
Locations/            # Site/location management
Shifts/               # Work shift management
FinancialTransactions/# Payroll and financial records
Statistics/           # Dashboard statistics aggregation
Reference/            # Lookup data (cities, types, nationalities)
Upload/               # File/image upload
Data/                 # DbContext, entities, DTOs, seeder
Common/               # Shared utilities, middleware, result pattern
Config/               # Settings POCOs
Services/             # MailService implementation
Migrations/           # EF Core migrations
```

## Getting Started

```bash
# Using Docker Compose (recommended)
docker-compose up -d

# Or manually
dotnet restore
dotnet run
```

API runs on `http://localhost:5231`. Swagger available at `/swagger` in Development.

## Configuration

Key environment variables (override `appsettings.json`):

| Variable | Description |
|---|---|
| `ConnectionStrings__DefaultConnection` | MySQL connection string |
| `JwtSettings__Key` | JWT signing secret (min 32 chars) |
| `JwtSettings__Issuer` | JWT issuer |
| `JwtSettings__Audience` | JWT audience |
| `CorsSettings__Origins__0` | Allowed CORS origin |
| `MailSettings__Host` | SMTP host |
| `MailSettings__Port` | SMTP port |
| `MailSettings__Username` | SMTP username |
| `MailSettings__Password` | SMTP password |

## API Endpoints

All endpoints are under `/api/dashboard/`.

### Auth
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/auth/login` | No | Login (returns JWT + sets refresh cookie) |
| POST | `/auth/register` | No | Register new user |
| POST | `/auth/refresh` | No | Refresh JWT token |
| POST | `/auth/logout` | Yes | Logout (invalidates all tokens) |
| POST | `/auth/verify-email` | No | Verify email with token |
| POST | `/auth/password/forgot` | No | Request password reset |
| POST | `/auth/password/reset` | No | Reset password |
| POST | `/auth/change-password` | Yes | Change password |
| GET | `/auth/profile` | Yes | Get current user profile |

### Users
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/users` | Yes | Paginated user list |
| GET | `/users/{id}` | Yes | User detail |
| POST | `/users` | Yes | Create user |
| PUT | `/users/{id}` | Yes | Update user |
| DELETE | `/users/{id}` | Yes | Soft-delete user |
| POST | `/users/users-export-excel` | Yes | Export to Excel |

### Attendance
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/attendance` | Yes | Paginated attendance records |
| POST | `/attendance/attendance-export-excel` | Yes | Export to Excel |

### Locations
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/locations` | Yes | Paginated location list |
| GET | `/locations/{id}` | Yes | Location detail |
| POST | `/locations` | Yes | Create location |
| PUT | `/locations/{id}` | Yes | Update location |
| DELETE | `/locations/{id}` | Yes | Soft-delete location |

### Statistics
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/statistics` | Yes | Dashboard statistics |

### Reference Data
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/reference/cities` | Yes | All cities |
| GET | `/reference/user-types` | Yes | All user types |
| GET | `/reference/nationalities` | Yes | All nationalities |

### Other
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/shifts` | Yes | Work shifts |
| GET | `/financial-transactions` | Yes | Financial records |
| POST | `/upload/image` | Yes | Upload image (max 5MB) |
| POST | `/upload/file` | Yes | Upload file (max 10MB) |
| GET | `/health` | No | Health check |
| GET | `/health/ready` | No | Readiness check (DB) |

## Rate Limiting

| Policy | Limit | Applied To |
|---|---|---|
| `global` | 100 req/min | All endpoints |
| `login` | 5 req/min | Login |
| `register` | 3 req/min | Register |
| `password_reset` | 3 req/min | Password reset |
| `otp_verify` | 5 req/min | OTP verification |
| `upload` | 10 req/min | File uploads |

## Deployment

Deployed on **Railway** with Docker. Multi-stage build:

```bash
docker build -t schools-league-api .
docker run -p 8080:8080 schools-league-api
```

### Docker Compose

```bash
docker-compose up -d
```

Starts API + MySQL with persistent volumes.

## Testing

```bash
dotnet test
```

Swagger UI available at `/swagger` in Development mode.
