using System.Text.Json;
using Common.Models;

namespace SchoolsLeague.Tests;

public class ResultTests
{
    [Fact]
    public void Ok_ShouldSetIsSuccessTrue()
    {
        var result = Result<string>.Ok("data");
        Assert.True(result.IsSuccess);
        Assert.Equal("data", result.Data);
    }

    [Fact]
    public void Fail_ShouldSetIsSuccessFalse()
    {
        var result = Result<string>.Fail("error");
        Assert.False(result.IsSuccess);
        Assert.Equal("error", result.Message);
    }

    [Fact]
    public void NotFound_ShouldSetCode404()
    {
        var result = Result<string>.NotFound();
        Assert.Equal(404, result.Code);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void ValidationError_ShouldSetCode422()
    {
        var result = Result<string>.ValidationError();
        Assert.Equal(422, result.Code);
    }

    [Fact]
    public void Serialization_UsesSnakeCase()
    {
        var result = Result<string>.Ok("hello", "Success", 200);
        var json = JsonSerializer.Serialize(result);

        Assert.Contains("is_success", json);
        Assert.Contains("\"data\":", json);
        Assert.Contains("\"message\":", json);
        Assert.Contains("\"code\":", json);
        Assert.DoesNotContain("IsSuccess", json);
    }
}

public class PaginatedResultSerializationTests
{
    [Fact]
    public void PaginatedResult_SerializesWithSnakeCase()
    {
        var pr = new PaginatedResult<string>
        {
            Data = ["a", "b"],
            TotalObjects = 2,
            PerPage = 5,
            CurrentPage = 1,
        };

        var json = JsonSerializer.Serialize(pr);

        Assert.Contains("is_success", json);
        Assert.Contains("i_per_page", json);
        Assert.Contains("i_total_objects", json);
        Assert.Contains("i_current_page", json);
        Assert.DoesNotContain("CurrentPage", json);
        Assert.DoesNotContain("IPerPage", json);
    }
}
