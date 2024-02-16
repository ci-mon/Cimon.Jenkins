using Cimon.Jenkins.Entities.Jobs;
using System.Collections.Generic;

namespace Cimon.Jenkins.Entities.Views;

public record ViewInfo : View
{
    public string? Description { get; set; }
    public IList<Job> Jobs { get; set; } = new List<Job>();
}
