using Cimon.Jenkins.Entities.Builds;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Cimon.Jenkins.Entities.Jobs;

public record JobInfo : Job
{
    public bool Buildable { get; set; }
    public bool Disabled { get; set; }
    public bool ConcurrentBuild { get; set; }
    public string Description { get; set; }
    public string DisplayName { get; set; }
    public string DisplayNameOrNull { get; set; }
    public bool InQueue { get; set; }
    public bool KeepDependencies { get; set; }
    public long NextBuildNumber { get; set; }
    public IList<HealthReport> HealthReport { get; set; } = new List<HealthReport>();
    public IList<Build> Builds { get; set; } = new List<Build>();
    public IList<Job> Jobs { get; set; } = new List<Job>();
    public Build FirstBuild { get; set; }
    public Build LastBuild { get; set; }
    public Build LastCompletedBuild { get; set; }
    public Build LastFailedBuild { get; set; }
    public Build LastStableBuild { get; set; }
    public Build LastSuccessfulBuild { get; set; }
    public Build LastUnstableBuild { get; set; }
    public Build LastUnsuccessfulBuild { get; set; }

    public JsonArray Actions { get; set; }
    public JsonArray Property { get; set; }

}
