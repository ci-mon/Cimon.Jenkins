using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Cimon.Jenkins;

public abstract record BaseQuery<T> : IQuery<T>
{
	public abstract string GetPath();
	public virtual bool AddApiJsonSuffix => true;
	protected virtual bool CheckStatusCode => true;

	public async Task<T?> GetResult(HttpResponseMessage response, CancellationToken ctx) {
		if (CheckStatusCode) {
			response.EnsureSuccessStatusCode();
		}
		return await OnGetResult(response, ctx);
	}

	protected virtual async Task<T?> OnGetResult(HttpResponseMessage response, CancellationToken ctx) {
		return await response.Content.ReadFromJsonAsync<T>(ctx).ConfigureAwait(false);
	}
}
