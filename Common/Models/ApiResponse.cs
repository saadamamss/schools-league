using System.Text.Json.Serialization;

namespace Common.Models;

public class ApiResponse<T>
{
    [JsonPropertyName("data")]
    public T? Data { get; set; }

    [JsonPropertyName("status")]
    public ApiStatus? Status { get; set; }

    [JsonPropertyName("pagination")]
    public PaginationMeta? Pagination { get; set; }

    [JsonPropertyName("statistics")]
    public Dictionary<string, object>? Statistics { get; set; }
}

public class ApiStatus
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("success")]
    public bool Success { get; set; }
}

public class PaginationMeta
{
    [JsonPropertyName("i_per_page")]
    public int IPerPage { get; set; }

    [JsonPropertyName("i_total_objects")]
    public int ITotalObjects { get; set; }

    [JsonPropertyName("i_current_page")]
    public int ICurrentPage { get; set; }
}

public class AppCodes
{
    public const int Created = 201;
    public const int Success = 200;
    public const int NotFound = 404;
    public const int Conflict = 409;
    public const int ValidationError = 422;
    public const int Forbidden = 403;
    public const int InvalidCredentials = 1001;
    public const int AccountLocked = 1002;
    public const int EmailNotVerified = 1003;
    public const int InvalidToken = 1004;
    public const int EmailAlreadyExists = 1005;
    public const int InternalError = 500;
}
