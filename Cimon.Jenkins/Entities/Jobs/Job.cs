using System;

namespace Cimon.Jenkins.Entities.Jobs;

public record Job
{
    public string Color { get; set; }
    public string Name { get; set; }
    public Uri Url { get; set; }

}
