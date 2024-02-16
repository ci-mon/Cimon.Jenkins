using System.Collections.Generic;

namespace Cimon.Jenkins.Entities.Builds;

public record ChangeSet
{
    public string? Kind { get; set; }
    public IList<ChangeSetItem> Items { get; set; } = new List<ChangeSetItem>();
    public IList<ChangeSetRevision> Revisions = new List<ChangeSetRevision>();
}
