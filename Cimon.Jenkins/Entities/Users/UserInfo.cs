using System.Collections.Generic;
using System.Text.Json.Nodes;
using Cimon.Jenkins.Entities.Builds;

namespace Cimon.Jenkins.Entities.Users;

public record UserInfo : User
{
    public string? Description { get; set; }
    public string? Id { get; set; }
    public List<DynamicItem> Property { get; set; }
}
