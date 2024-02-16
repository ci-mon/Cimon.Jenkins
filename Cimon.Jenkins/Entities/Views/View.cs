using System;

namespace Cimon.Jenkins.Entities.Views;

public record View
{
    public string? Name { get; set; }
    public Uri? Url { get; set; }

}
