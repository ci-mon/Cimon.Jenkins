namespace Cimon.Jenkins.WorkflowApi;

using System.Collections.Generic;
using System.Text.Json.Serialization;

public class StageFlowNode
{
	[JsonPropertyName("_links")]
	public StageFlowNodeLinks Links { get; set; }
	public string Id { get; set; }
	public string Name { get; set; }
	public string ExecNode { get; set; }
	public string Status { get; set; }
	public long StartTimeMillis { get; set; }
	public long DurationMillis { get; set; }
	public long PauseDurationMillis { get; set; }
	public List<string> ParentNodes { get; set; }
	public Error? Error { get; set; }
	public string ParameterDescription { get; set; }

	public JenkinsWfApi.GetFlowNodeLog GetLogQuery() => new(Links.Log.Href);
}