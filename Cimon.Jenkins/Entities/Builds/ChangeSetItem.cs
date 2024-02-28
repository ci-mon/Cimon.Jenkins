using Cimon.Jenkins.Entities.Users;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cimon.Jenkins.Entities.Builds;

public record ChangeSetItem: BaseItem
{
    public string CommitId { get; set; }
    public string ShortCommitId => CommitId.Length > 8 ? CommitId.Substring(0, 8) : CommitId;
    [JsonConverter(typeof(DateTimeISO8601JsonConverter))]
    public DateTime Date { get; set; }
    public long Timestamp { get; set; }
    [JsonPropertyName("msg")]
    public string Message { get; set; }
    public User Author { get; set; }
    public IList<string> AffectedPaths { get; set; } = new List<string>();
    public IList<ChangeSetPath> Paths { get; set; } = new List<ChangeSetPath>();

    public override string ToString() => $"{ShortCommitId}: {Message} - {Author}";
}
