using System;

namespace Cimon.Jenkins.Entities.Builds;

using System.Linq;

public record Build : Entity
{
    public long Number { get; set; }
    public Uri? Url { get; set; }
	public JobLocator Locator {
		get {
			var path = Url?.GetComponents(UriComponents.Path, UriFormat.Unescaped);
			if (path is null) return default;
			var parts = path.Split(new[]{'/'}, StringSplitOptions.RemoveEmptyEntries).AsSpan()[..^1];
			return JobLocator.Create(parts.ToArray().Where(x => !"job".Equals(x, StringComparison.OrdinalIgnoreCase))
				.ToArray());
		}
	}

}
