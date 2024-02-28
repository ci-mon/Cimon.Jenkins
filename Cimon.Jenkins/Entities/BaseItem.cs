using System.Text.Json.Serialization;

namespace Cimon.Jenkins.Entities;

public record BaseItem
{
	[JsonPropertyName("_class")]
	public string Class { get; set; }
}
