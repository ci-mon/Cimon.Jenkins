using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Cimon.Jenkins;

public abstract record Query<T> : BaseRequest
{
	public virtual string? ApiSuffix => "/api/json";

	public async Task<T?> GetResult(HttpResponseMessage response, CancellationToken ctx) {
		if (response.StatusCode == HttpStatusCode.NotFound) {
			return default;
		}
		await CheckResponse(response, ctx).ConfigureAwait(false);
		return await OnGetResult(response, ctx).ConfigureAwait(false);
	}

	protected virtual async Task<T?> OnGetResult(HttpResponseMessage response, CancellationToken ctx) {
		return await response.Content.ReadFromJsonAsync<T>(ctx).ConfigureAwait(false);
	}
}
