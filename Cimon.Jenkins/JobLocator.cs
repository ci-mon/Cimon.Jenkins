using System;
using System.Linq;

namespace Cimon.Jenkins;

public class JobLocator
{
	private JobLocator() {
	}
	private string[] _path = Array.Empty<string>();
	public static JobLocator Create(params string[] jobs) => new() {
		_path = jobs.ToArray()
	};

	public override string ToString() {
		return string.Join("/", _path.Select(x => $"job/{x}"));
	}
}