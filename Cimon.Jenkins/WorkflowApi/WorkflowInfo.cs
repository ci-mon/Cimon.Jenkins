namespace Cimon.Jenkins.WorkflowApi;

using System.Collections.Generic;
using System.Text.Json.Serialization;

public class WorkflowInfo
{
	[JsonPropertyName("_links")]
	public WorkflowLinks Links { get; set; }
	public string Id { get; set; }
	public string Name { get; set; }
	public string Status { get; set; }
	public long StartTimeMillis { get; set; }
	public long EndTimeMillis { get; set; }
	public long DurationMillis { get; set; }
	public long QueueDurationMillis { get; set; }
	public long PauseDurationMillis { get; set; }
	public List<Stage> Stages { get; set; }
}