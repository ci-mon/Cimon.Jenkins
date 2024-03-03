using System;
using System.Diagnostics;
using System.Linq;

namespace Cimon.Jenkins;

[DebuggerDisplay("Items={PathItems.Length} {ToString()}")]
public struct JobLocator
{
	private string[] _path;
	private string[] PathItems => _path ?? Array.Empty<string>();

	public static JobLocator Create(params string[] jobs) => new() {
		_path = jobs.ToArray()
	};

	public override string ToString() => string.Join("/", PathItems.Select(x => $"job/{x}"));

	public string Name => _path?.Length > 0 ? PathItems[^1] : string.Empty;
	public string Path => string.Join("/", PathItems);

	public void Deconstruct(out string name, out JobLocator path) {
		name = Name;
		path = _path?.Length > 1 ? Create(PathItems[..^1]) : new JobLocator();
	}

	public static implicit operator JobLocator(string name) => Create(name);
}
