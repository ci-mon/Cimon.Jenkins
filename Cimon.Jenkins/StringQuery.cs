using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Cimon.Jenkins;

public abstract record StringQuery : Query<string>
{
	public override string? ApiSuffix => null;
	protected override async Task<string?> OnGetResult(HttpResponseMessage response, CancellationToken ctx) =>
		await response.Content.ReadAsStringAsync(ctx).ConfigureAwait(false);
}

#if NETSTANDARD2_0
internal static class Polyfills
{
	public static Task<string> ReadAsStringAsync(this HttpContent message, CancellationToken token) {
		return message.ReadAsStringAsync();
	}
}
#endif
