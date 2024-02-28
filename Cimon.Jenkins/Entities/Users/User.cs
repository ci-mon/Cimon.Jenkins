using System;

namespace Cimon.Jenkins.Entities.Users;

public record User: BaseItem
{
    public Uri? AbsoluteUrl { get; set; }
    public string? FullName { get; set; }

}
