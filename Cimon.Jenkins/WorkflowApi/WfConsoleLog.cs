namespace Cimon.Jenkins.WorkflowApi;

public class WfConsoleLog
{
	public string NodeId { get; set; }
	public string NodeStatus { get; set; }
	public int Length { get; set; }
	public bool HasMore { get; set; }
	public string Text { get; set; }
	public string ConsoleUrl { get; set; }
}