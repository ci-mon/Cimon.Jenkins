using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Cimon.Jenkins;

public abstract record BaseRequest
{
	public abstract string GetPath();
	protected virtual bool CheckStatusCode => true;
	public virtual HttpMethod Method => HttpMethod.Get;
	protected virtual HttpContent? GetContent() => null;
	public HttpContent? Content => GetContent();
	protected Task CheckResponse(HttpResponseMessage response, CancellationToken ctx) {
		if (CheckStatusCode) {
			response.EnsureSuccessStatusCode();
		}
		return Task.CompletedTask;
	}

}
