namespace Cimon.Jenkins.WorkflowApi;

using System.Collections.Generic;
using System.Text.Json.Serialization;

public record Stage : BaseWfApiQuery<Stage>
{
	[JsonPropertyName("_links")]
	public Links Links { get; set; }
	public string Id { get; set; }
	public string Name { get; set; }
	public string ExecNode { get; set; }
	public string Status { get; set; }
	public long StartTimeMillis { get; set; }
	public long DurationMillis { get; set; }
	public long PauseDurationMillis { get; set; }
	public Error? Error { get; set; }
	public List<StageFlowNode> StageFlowNodes { get; set; }

	public override string GetPath() => Links.Self.Href;
	public override string? ApiSuffix => null;

}