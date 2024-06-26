﻿using Cimon.Jenkins.Entities.Users;
using System.Collections.Generic;
using System.Text.Json.Nodes;
namespace Cimon.Jenkins.Entities.Builds;

public record BuildInfo : Build
{
    public bool Building { get; set; }
    public string? BuiltOn { get; set; }
    public string? Description { get; set; }
    public string? DisplayName { get; set; }
    public long Duration { get; set; }
    public long EstimatedDuration { get; set; }
    public DynamicItem? Executor { get; set; }
    public string? FullDisplayName { get; set; }
    public required string Id { get; set; }
    public bool KeepLog { get; set; }
    public long QueueId { get; set; }
    public long Timestamp { get; set; }
    public string? Result { get; set; }
    public List<ChangeSet>? ChangeSets { get; set; }
    public IList<User> Culprits { get; set; } = new List<User>();
    public List<DynamicItem>? Actions { get; set; }
    public bool InProgress { get; set; }
    public Build NextBuild { get; set; }
    public Build PreviousBuild { get; set; }

	public JenkinsApi.BuildInfo ToQuery() => new(Id, Locator);
	public JenkinsApi.TestsReport QueryTests() => new JenkinsApi.TestsReport(ToQuery());
}
