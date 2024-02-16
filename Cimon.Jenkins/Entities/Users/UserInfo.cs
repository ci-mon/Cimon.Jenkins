using System.Text.Json.Nodes;

namespace Cimon.Jenkins.Entities.Users;

public record UserInfo : User
{
    public string? Description { get; set; }
    public string? Id { get; set; }

    public JsonArray? Property { get; set; }
}
