using System;

namespace Cimon.Jenkins.Entities.Builds;

public record Build : Entity
{
    public long Number { get; set; }
    public Uri? Url { get; set; }

}
