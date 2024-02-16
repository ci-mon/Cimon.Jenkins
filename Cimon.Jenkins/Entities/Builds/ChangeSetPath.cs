using System.Text.Json.Serialization;

namespace Cimon.Jenkins.Entities.Builds;

public record ChangeSetPath
{
    [JsonConverter(typeof(JsonStringEnumConverter<EditType>))]
    public EditType EditType { get; set; }
    public string? File { get; set; }
}
