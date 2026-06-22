using System.Text.Json.Serialization;

namespace Common.Models;

public class Result<T>
{
    [JsonPropertyName("is_success")]
    public bool IsSuccess { get; set; }

    [JsonPropertyName("data")]
    public T? Data { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("code")]
    public int Code { get; set; }

    public static Result<T> Ok(T data, string message = "Success", int code = 200) =>
        new() { IsSuccess = true, Data = data, Message = message, Code = code };

    public static Result<T> Fail(string message, int code = 400) =>
        new() { IsSuccess = false, Message = message, Code = code };

    public static Result<T> NotFound(string message = "Resource not found.") =>
        new() { IsSuccess = false, Message = message, Code = 404 };

    public static Result<T> Conflict(string message = "Conflict.") =>
        new() { IsSuccess = false, Message = message, Code = 409 };

    public static Result<T> ValidationError(string message = "Validation failed.") =>
        new() { IsSuccess = false, Message = message, Code = 422 };

    public static Result<T> Unauthorized(string message = "Unauthorized.") =>
        new() { IsSuccess = false, Message = message, Code = 401 };
}

public class PaginatedResult<T>
{
    [JsonPropertyName("is_success")]
    public bool IsSuccess { get; set; } = true;

    [JsonPropertyName("data")]
    public List<T> Data { get; set; } = [];

    [JsonPropertyName("message")]
    public string Message { get; set; } = "Success";

    [JsonPropertyName("code")]
    public int Code { get; set; } = 200;

    [JsonPropertyName("i_total_objects")]
    public int TotalObjects { get; set; }

    [JsonPropertyName("i_per_page")]
    public int PerPage { get; set; }

    [JsonPropertyName("i_current_page")]
    public int CurrentPage { get; set; }
}
