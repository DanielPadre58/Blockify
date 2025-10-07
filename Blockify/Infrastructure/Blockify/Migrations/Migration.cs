using System.Text.Json.Serialization;

namespace Blockify.Infrastructure.Blockify.Migrations;

public class Migration
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    [JsonPropertyName("version")]
    public required int Version { get; set; }
    [JsonPropertyName("sql")]
    public required string Sql { get; set; }
}
