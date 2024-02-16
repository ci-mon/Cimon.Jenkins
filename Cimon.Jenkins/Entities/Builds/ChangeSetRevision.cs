namespace Cimon.Jenkins.Entities.Builds;

public record ChangeSetRevision
{
    public string? Module { get; set; }
    public string? Revision { get; set; }
}
