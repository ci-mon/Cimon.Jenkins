using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.Extensions.Http;

namespace Cimon.Jenkins;

public class JenkinsClient : IJenkinsClient
{
	private readonly HttpClient _httpClient;
	private readonly JenkinsConfig _jenkinsConfig;

	public JenkinsClient(JenkinsConfig jenkinsConfig, IHttpClientFactory httpClientFactory) {
#if !NETSTANDARD2_0
		ArgumentNullException.ThrowIfNull(jenkinsConfig);
#endif

		_httpClient = httpClientFactory.CreateClient(nameof(JenkinsClient));
		_httpClient.BaseAddress = jenkinsConfig.JenkinsUrl;

		_jenkinsConfig = jenkinsConfig;

		if (string.IsNullOrEmpty(jenkinsConfig.Username) || string.IsNullOrEmpty(jenkinsConfig.ApiKey)) return;
		var byteArray = Encoding.ASCII.GetBytes(jenkinsConfig.Username + ':' + jenkinsConfig.ApiKey);
		_httpClient.DefaultRequestHeaders.Authorization =
			new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
	}

	public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() {
		return HttpPolicyExtensions
			.HandleTransientHttpError()
			.OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
			.WaitAndRetryForeverAsync((retryAttempt, context) => {
					context["Attempt"] = retryAttempt;
					if (context["Config"] is JenkinsConfig config) {
						return TimeSpan.FromSeconds(Math.Pow(config.RetryBackoffExponent, retryAttempt));
					}
					return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
				},
				(res, _, context) => {
					context["LastResult"] = (object)res.Exception ?? res.Result;
					if (context["Attempt"] is not int attempt || context["Config"] is not JenkinsConfig config ||
						context[nameof(CancellationTokenSource)] is not CancellationTokenSource tokenSource)
						throw new NotSupportedException();
					if (attempt >= config.RetryAttempts) {
						tokenSource.Cancel();
					}
				});
	}

	public async Task<TResult?> Query<TResult>(Query<TResult> query, CancellationToken token = default) {
		var path = query.GetPath();
		if (query.AddApiJsonSuffix) {
			path = $"{path}/api/json";
		}
		using var request = new HttpRequestMessage(query.Method, path);
		using var response = await RunWithPolicy(token, request);
		return await query.GetResult(response, token).ConfigureAwait(false);
	}

	public async Task Exec(Command command, CancellationToken token = default) {
		var path = command.GetPath();
		using var request = new HttpRequestMessage(command.Method, path);
		if (command.Content is { } content) {
			request.Content = content;
		}
		using var response = await RunWithPolicy(token, request);
		using var redirectedResponse = await FollowRedirect(response, token);
		await command.Verify(redirectedResponse, token).ConfigureAwait(false);
	}

	private async Task<HttpResponseMessage> FollowRedirect(HttpResponseMessage response, CancellationToken token) {
		if (response.StatusCode != HttpStatusCode.Redirect ||
				response.Headers.Location?.AbsoluteUri is not {Length: > 0} url) {
			return response;
		}
		return await RunWithPolicy(token, new HttpRequestMessage(HttpMethod.Get, url));
	}

	private async Task<HttpResponseMessage> RunWithPolicy(CancellationToken ctx,
		HttpRequestMessage request) {
		var context = new Context { { "Config", _jenkinsConfig } };
		using var cts = new CancellationTokenSource();
		context.Add(nameof(CancellationTokenSource), cts);
		request.SetPolicyExecutionContext(context);
		var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, ctx);
		try {
			return await _httpClient.SendAsync(request, combinedCts.Token).ConfigureAwait(false);
		} catch (OperationCanceledException) when (context.TryGetValue("LastResult", out var r)
				&& r is Exception exception) {
			throw exception;
		} catch (OperationCanceledException) when (context.TryGetValue("LastResult", out var r)
				&& r is HttpResponseMessage response) {
			return response;
		}
	}

	public void Dispose() => _httpClient.Dispose();
}
