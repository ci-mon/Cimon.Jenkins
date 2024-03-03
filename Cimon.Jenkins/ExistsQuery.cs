using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Cimon.Jenkins;

public record ExistsQuery<T>(Query<T> SourceQuery) : Query<bool>
{
	protected override bool CheckStatusCode => false;

	public override string GetPath() => SourceQuery.GetPath();

	protected override Task<bool> OnGetResult(HttpResponseMessage response, CancellationToken ctx) =>
		Task.FromResult(response.IsSuccessStatusCode);
}
