using Cimon.Jenkins.Entities.Jobs;
using Cimon.Jenkins.Entities.Views;
using System.Collections.Generic;

namespace Cimon.Jenkins.Entities;

public record Master : BaseItem
{
    public string Mode { get; set; }
    public string NodeDescription { get; set; }
    public string NodeName { get; set; }
    public int NumExecutors { get; set; }
    public bool QuietingDown { get; set; }
    public string? QuietDownReason { get; set; }
    public int SlaveAgentPort { get; set; }
    public bool UseCrumbs { get; set; }
    public bool UseSecurity { get; set; }
    public View PrimaryView { get; set; }
    public IList<Job> Jobs { get; set; } = new List<Job>();
    public IList<View> Views { get; set; } = new List<View>();
}
