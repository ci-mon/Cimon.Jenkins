using System;

namespace Cimon.Jenkins.Entities.Views;

public record View : IQueryProvider
{
    public string? Name { get; set; }
    public required Uri Url { get; set; }
    public IQuery<ViewInfo> ToQuery() => new Query.View(Name!);
}
