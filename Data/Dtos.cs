using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Data;

public class LoginDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = string.Empty;
}

public class RegisterDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number.")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
    public string Password { get; set; } = string.Empty;

    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string PasswordConfirmation { get; set; } = string.Empty;

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
    public string FirstName { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Middle name cannot exceed 100 characters.")]
    public string? MiddleName { get; set; }

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "SA ID is required.")]
    [StringLength(20, ErrorMessage = "SA ID cannot exceed 20 characters.")]
    public string SaId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Gender is required.")]
    public string Gender { get; set; } = string.Empty;

    public int? CityId { get; set; }
    public int? UserTypeId { get; set; }
}

public class VerifyRegistrationDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Verification token is required.")]
    [JsonPropertyName("otp")]
    public string Token { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
    /// <summary>Internal: the controller reads this to set the HttpOnly cookie. Not serialized.</summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public string? RefreshToken { get; set; }
}

public class ForgotPasswordDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Reset token is required.")]
    [JsonPropertyName("otp")]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
    public string Password { get; set; } = string.Empty;

    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    [JsonPropertyName("password_confirmation")]
    public string PasswordConfirmation { get; set; } = string.Empty;
}

public class ChangePasswordDto
{
    [Required(ErrorMessage = "Current password is required.")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "New password is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
    public string NewPassword { get; set; } = string.Empty;

    [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
    public string NewPasswordConfirmation { get; set; } = string.Empty;
}

public class ResendVerificationDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = string.Empty;
}

public class VerifyOtpDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "OTP is required.")]
    public string Otp { get; set; } = string.Empty;
}

public class RefreshRequestDto
{
    public string? Token { get; set; }
}

public class VerifyEmailQuery
{
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// Full user DTO with bank info — use for profile and single-user detail endpoints.
/// </summary>
public class UserDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? SaId { get; set; }
    public string? Gender { get; set; }
    public string? ProfileImage { get; set; }
    public bool IsActive { get; set; }
    public DateTime? BirthDate { get; set; }
    public CityDto? City { get; set; }
    public UserTypeDto? UserType { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string? TodayAttendanceStatus { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string? TodayCheckIn { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string? TodayCheckOut { get; set; }
    public int WorkingDaysCount { get; set; }
    public double MonthlyWorkingHours { get; set; }
    public decimal DailyRate { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Permissions { get; set; } = [];
    public List<LocationDto> Locations { get; set; } = [];
    public BankInfoDto? BankInfo { get; set; }
}

/// <summary>
/// Summary user DTO (no bank info) — use for list endpoints and nested references.
/// </summary>
public class UserSummaryDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? SaId { get; set; }
    public string? Gender { get; set; }
    public string? ProfileImage { get; set; }
    public bool IsActive { get; set; }
    public DateTime? BirthDate { get; set; }
    public CityDto? City { get; set; }
    public UserTypeDto? UserType { get; set; }
    public decimal DailyRate { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Permissions { get; set; } = [];
}

public class BankInfoDto
{
    public string? BankName { get; set; }
    public string? AccountNumber { get; set; }
    public string? Iban { get; set; }
    public string? SwiftCode { get; set; }
}

public class CityDto
{
    public int Id { get; set; }
    public Dictionary<string, string> Name { get; set; } = [];
}

public class UserTypeDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public Dictionary<string, string> Name { get; set; } = [];
    public string Key { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class CreateUserDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number.")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
    public string FirstName { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Middle name cannot exceed 100 characters.")]
    public string? MiddleName { get; set; }

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "SA ID is required.")]
    [StringLength(20, ErrorMessage = "SA ID cannot exceed 20 characters.")]
    public string SaId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Gender is required.")]
    public string Gender { get; set; } = string.Empty;

    public int? CityId { get; set; }
    public int? UserTypeId { get; set; }
    public int? NationalityId { get; set; }
    public List<string> Roles { get; set; } = [];
}

public class UpdateUserDto
{
    [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
    public string? FirstName { get; set; }

    [StringLength(100, ErrorMessage = "Middle name cannot exceed 100 characters.")]
    public string? MiddleName { get; set; }

    [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
    public string? LastName { get; set; }

    [Phone(ErrorMessage = "Invalid phone number.")]
    public string? Phone { get; set; }

    public string? Gender { get; set; }
    public int? CityId { get; set; }
    public int? UserTypeId { get; set; }
    public int? NationalityId { get; set; }
    public bool? IsActive { get; set; }
    public List<string>? Roles { get; set; }
}

public class AttendanceRecordDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int LocationId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? CheckIn { get; set; }
    public string? CheckOut { get; set; }
    public double? TotalHours { get; set; }
    public string Date { get; set; } = string.Empty;
    public UserSummaryDto User { get; set; } = null!;
    public LocationDto Location { get; set; } = null!;
}

public class LocationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Type { get; set; }
    public string? License { get; set; }
    public string? Phone { get; set; }
    public int? JoinedEmployee { get; set; }
    public int? ObserverCount { get; set; }
    public string? WorkHours { get; set; }
    public CityDto? City { get; set; }
    public bool IsActive { get; set; }
    public string? Image { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Address { get; set; }
    public int? CurrentEventsCount { get; set; }
    public double? TotalWorkingHours { get; set; }
    public DateTime? AssignedAt { get; set; }
    public DateTime? UnassignedAt { get; set; }
}

public class ShiftDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public int? LocationId { get; set; }
    public bool IsActive { get; set; }
}

public class UserLocationAssignmentDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int LocationId { get; set; }
    public string? Role { get; set; }
    public DateTime AssignedAt { get; set; }
    public UserSummaryDto? User { get; set; }
    public LocationDto? Location { get; set; }
}

public class FinancialTransactionDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? PaymentTypeId { get; set; }
    public string? DeductionBasis { get; set; }
    public string? DeductionValue { get; set; }
    public DateTime TransactionDate { get; set; }
    public UserSummaryDto? User { get; set; }
    public PaymentTypeDto? PaymentType { get; set; }
}

public class PaymentTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

// ── Location DTOs ──────────────────────────────────────────────────
public class CreateLocationDto
{
    [Required(ErrorMessage = "Site name is required.")]
    [StringLength(200, ErrorMessage = "Site name cannot exceed 200 characters.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Type { get; set; }

    [StringLength(100)]
    public string? License { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int? CityId { get; set; }
    public string? Image { get; set; }
    public int? JoinedEmployee { get; set; }
    public string? WorkHours { get; set; }
    public string? WorkType { get; set; }
}

public class UpdateLocationDto
{
    [StringLength(200)]
    public string? Name { get; set; }

    [StringLength(100)]
    public string? Type { get; set; }

    [StringLength(100)]
    public string? License { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int? CityId { get; set; }
    public string? Image { get; set; }
    public int? JoinedEmployee { get; set; }
    public string? WorkHours { get; set; }
    public string? WorkType { get; set; }
    public bool? IsActive { get; set; }
}

public class LocationListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Type { get; set; }
    public string? License { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public int JoinedEmployee { get; set; }
    public int ObserverCount { get; set; }
    public int EmployeeNumber { get; set; }
    public int ObserverNumber { get; set; }
    public string WorkHours { get; set; } = "0";
    public string? Image { get; set; }
    public bool IsActive { get; set; }
    public CityDto? City { get; set; }
    public int CurrentEventsCount { get; set; }
}

public class EmployeeBriefDto
{
    public string FullName { get; set; } = string.Empty;
    public UserTypeDto? UserType { get; set; }
    public CityDto? City { get; set; }
    public double MonthlyWorkingHours { get; set; }
    public string? TodayCheckIn { get; set; }
    public string? TodayCheckOut { get; set; }
}

public class LocationDetailDto
{
    public string Name { get; set; } = string.Empty;
    public string? Type { get; set; }
    public string? LicenseNo { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public int EmployeeNumber { get; set; }
    public int ObserverNumber { get; set; }
    public int ObserverCount { get; set; }
    public string WorkHours { get; set; } = "0";
    public double TotalWorkingHours { get; set; }
    public int MaxUsers { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Image { get; set; }
    public EmployeeBriefDto? Supervisor { get; set; }
    public List<EmployeeBriefDto> Inspectors { get; set; } = [];
}

// ── Shift DTOs ─────────────────────────────────────────────────────
public class CreateShiftDto
{
    [Required(ErrorMessage = "Shift name is required.")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }

    public int? LocationId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateShiftDto
{
    [StringLength(200)]
    public string? Name { get; set; }

    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public int? LocationId { get; set; }
    public bool? IsActive { get; set; }
}

// ── FinancialTransaction DTOs ───────────────────────────────────────
public class CreateFinancialTransactionDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Type is required.")]
    [StringLength(50)]
    public string Type { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int? PaymentTypeId { get; set; }

    [StringLength(50)]
    public string? DeductionBasis { get; set; }

    [StringLength(100)]
    public string? DeductionValue { get; set; }
}

// ── UserLocationAssignment DTOs ─────────────────────────────────────
public class CreateUserLocationAssignmentDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int LocationId { get; set; }

    [StringLength(50)]
    public string? Role { get; set; }
}

public class UpdateUserLocationAssignmentDto
{
    [StringLength(50)]
    public string? Role { get; set; }
}
