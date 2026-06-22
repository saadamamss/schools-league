using System.Text.Json;
using Common.Models;

namespace SchoolsLeague.Tests;

public class PaginationMetaTests
{
    [Fact]
    public void Serialization_UsesSnakeCasePropertyNames()
    {
        var meta = new PaginationMeta
        {
            IPerPage = 10,
            ITotalObjects = 100,
            ICurrentPage = 2,
        };

        var json = JsonSerializer.Serialize(meta);
        var parsed = JsonDocument.Parse(json);

        Assert.True(parsed.RootElement.TryGetProperty("i_per_page", out _));
        Assert.True(parsed.RootElement.TryGetProperty("i_total_objects", out _));
        Assert.True(parsed.RootElement.TryGetProperty("i_current_page", out _));

        Assert.Equal(10, parsed.RootElement.GetProperty("i_per_page").GetInt32());
        Assert.Equal(100, parsed.RootElement.GetProperty("i_total_objects").GetInt32());
        Assert.Equal(2, parsed.RootElement.GetProperty("i_current_page").GetInt32());
    }

    [Fact]
    public void Deserialization_AcceptsSnakeCase()
    {
        var json = """{"i_per_page":25,"i_total_objects":500,"i_current_page":3}""";
        var meta = JsonSerializer.Deserialize<PaginationMeta>(json);

        Assert.NotNull(meta);
        Assert.Equal(25, meta.IPerPage);
        Assert.Equal(500, meta.ITotalObjects);
        Assert.Equal(3, meta.ICurrentPage);
    }
}
