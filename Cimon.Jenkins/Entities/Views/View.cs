using System;

namespace Cimon.Jenkins.Entities.Views;

public record View : IQueryProvider
{
    public string? Name { get; set; }
    public required Uri Url { get; set; }
    public Query<ViewInfo> ToQuery() => new API.View(Name!);
}
