using System.Collections.Generic;
using System.Text.Json;

namespace Cimon.Jenkins.Entities.Builds;

public record TestCase : BaseTestInfo
{
	public int Age { get; set; }
	public int FailedSince { get; set; }
	public string ClassName { get; set; }
	public string ErrorDetails { get; set; }
	public string ErrorStackTrace { get; set; }
	public string SkippedMessage { get; set; }
	public bool Skipped { get; set; }
	public string Status { get; set; }
}

public record BaseTestInfo
{
	
	public List<DynamicItem> TestActions { get; set; }
	
	public string Name { get; set; }
	public string Stderr { get; set; }
	public string Stdout { get; set; }
	public float Duration { get; set; }
	public JsonElement Properties { get; set; }
}

public record TestSuite : BaseTestInfo
{
	public List<TestCase> Cases { get; set; }
	
	public string Id { get; set; }
	public string NodeId { get; set; }
	public long? Timestamp { get; set; }
	
	/*

      "enclosingBlockNames": [],
      "enclosingBlocks": [],
	 */
}

public record TestReport : BaseItem
{
	public long FailCount { get; set; }
	public long PassCount { get; set; }
	public long SkipCount { get; set; }
	public bool Empty { get; set; }
	public float Duration { get; set; }
	public List<TestSuite> Suites { get; set; }
}
