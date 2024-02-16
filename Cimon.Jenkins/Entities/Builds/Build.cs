using System;

namespace Cimon.Jenkins.Entities.Builds;

public record Build
{
    public long Number { get; set; }
    public Uri? Url { get; set; }

}
