using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Cimon.Jenkins.Entities.Builds;

[JsonConverter(typeof(DynamicItemConverter))]
[DebuggerDisplay("{Class}, {Props}")]
public record DynamicItem : BaseItem
{
	public Dictionary<string, object?> Props { get; set; } = new();
}
