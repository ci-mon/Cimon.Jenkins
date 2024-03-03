using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Cimon.Jenkins;

public abstract record Command : BaseRequest
{
	public override HttpMethod Method => HttpMethod.Post;

	public async Task Verify(HttpResponseMessage response, CancellationToken token) {
		await CheckResponse(response, token).ConfigureAwait(false);
	}

	public override string GetPath() => $"/{ToLowerCamelCase(GetType().Name)}";

	protected string ToLowerCamelCase(string source) {
		if (source is not { Length: > 1 }) return source.ToLowerInvariant();
#if NETSTANDARD2_0
		return $"{Char.ToLowerInvariant(source[0])}{source.Substring(1)}";
#else
		return string.Create(source.Length, source, (span, s) => {
			span[0] = char.ToLowerInvariant(s[0]);
			for (int i = 1; i < s.Length; i++) {
				span[i] = source[i];
			}
		});
#endif
	}
}
