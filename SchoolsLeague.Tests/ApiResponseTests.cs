using System.Text.Json;
using Common.Models;

namespace SchoolsLeague.Tests;

public class ApiResponseTests
{
    [Fact]
    public void ApiResponse_SerializesCorrectly()
    {
        var response = new ApiResponse<object>
        {
            Data = new { id = 1 },
            Status = new ApiStatus { Message = "OK", Code = 200, Success = true },
        };

        var json = JsonSerializer.Serialize(response);
        var parsed = JsonDocument.Parse(json);

        Assert.True(parsed.RootElement.TryGetProperty("data", out _));
        Assert.True(parsed.RootElement.TryGetProperty("status", out _));
    }
}
