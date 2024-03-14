using System;

namespace Cimon.Jenkins.Entities.Users;

public record User: BaseItem
{
    public Uri? AbsoluteUrl { get; set; }
    public string? FullName { get; set; }

	public string? UserId {
		get {
			var path = AbsoluteUrl?.GetComponents(UriComponents.Path, UriFormat.Unescaped);
			return path?.Substring(path.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) + 1);
		}
	}

}
