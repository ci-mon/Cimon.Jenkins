using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cimon.Jenkins.Entities.Users;
using Polly;
using Polly.Extensions.Http;

namespace Cimon.Jenkins;

public class JenkinsClient : IJenkinsClient
{
    private readonly HttpClient _httpClient;
    private readonly JenkinsConfig _jenkinsConfig;
    public JenkinsClient(JenkinsConfig jenkinsConfig, IHttpClientFactory httpClientFactory)
    {
        ArgumentNullException.ThrowIfNull(jenkinsConfig);

        _httpClient = httpClientFactory.CreateClient(nameof(JenkinsClient));
        _httpClient.BaseAddress = jenkinsConfig.JenkinsUrl;

        _jenkinsConfig = jenkinsConfig;

        if (!string.IsNullOrEmpty(jenkinsConfig.Username) && !string.IsNullOrEmpty(jenkinsConfig.ApiKey))
        {
            var byteArray = Encoding.ASCII.GetBytes(jenkinsConfig.Username + ':'  + jenkinsConfig.ApiKey);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }
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
                        context[nameof(CancellationTokenSource)] is not CancellationTokenSource tokenSource) throw new NotSupportedException();
                    if (attempt >= config.RetryAttempts) {
                        tokenSource.Cancel();
                    }
                });
    }

    public async Task<TResult?> Get<TResult>(IQuery<TResult> query, CancellationToken ctx = default) {
        var path = query.GetPath();
        if (query.AddApiJsonSuffix) {
            path = $"{path}/api/json";
        }
        using var request = new HttpRequestMessage(query.Method, path);
        using var response = await RunWithPolicy(ctx, request);
        return await query.GetResult(response, ctx).ConfigureAwait(false);
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
        } catch (OperationCanceledException) when(context.TryGetValue("LastResult", out var r) && r is Exception exception) {
            throw exception;
        } catch (OperationCanceledException) when(context.TryGetValue("LastResult", out var r) && r is HttpResponseMessage response) {
            return response;
        }
    }
    
/*
   
    public async Task BuildProject(string job, CancellationToken ctx = default(CancellationToken))
    {
        var response = await GetRetryPolicy().ExecuteAsync(() => httpClient.PostAsync(jenkinsConfig.JenkinsUrl + JobPath(job) + "/build", null, ctx));

        response.EnsureSuccessStatusCode();
    }

    public async Task BuildProjectWithParameters(string job, IDictionary<string, string> parameters, CancellationToken ctx = default(CancellationToken))
    {
        var response = await GetRetryPolicy().ExecuteAsync(() =>
        {
            var p = new {parameter = parameters.Select(x => new {name = x.Key, value = x.Value}).ToArray()};
            var json = JsonConvert.SerializeObject(p);
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("json", json)
            });

            return httpClient.PostAsync(jenkinsConfig.JenkinsUrl + JobPath(job) + "/build", content, ctx);
        });

        response.EnsureSuccessStatusCode();
    }

    public async Task CopyJob(string fromJobName, string newJobName, CancellationToken ctx = default(CancellationToken))
    {
        await CopyJob(fromJobName, newJobName, "", ctx);
    }

    public async Task CopyJob(string fromJobName, string newJobName, string path, CancellationToken ctx = default(CancellationToken))
    {
        var requestUri = jenkinsConfig.JenkinsUrl + JobPath(path) + "/createItem?name=" + newJobName + "&mode=copy&from=" + fromJobName;
        var content = new StringContent("", Encoding.UTF8, "application/xml");

        HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = content
        };
        
        var response = await GetRetryPolicy().ExecuteAsync(() => httpClient.SendAsync(message, ctx));

        response = await FollowRedirect(response, ctx);
            
        response.EnsureSuccessStatusCode();
    }

    private async Task<HttpResponseMessage> FollowRedirect(HttpResponseMessage response, CancellationToken ctx)
    {
        if (response.StatusCode != HttpStatusCode.Redirect) return response;

        return await GetRetryPolicy().ExecuteAsync(() => httpClient.GetAsync(response.Headers.Location.AbsoluteUri, ctx));
    }

    public async Task UploadJobConfig(string job, string xml, CancellationToken ctx = default(CancellationToken))
    {
        var content = new StringContent(xml, Encoding.UTF8, "application/xml");

        var response = await GetRetryPolicy().ExecuteAsync(() => httpClient.PostAsync(jenkinsConfig.JenkinsUrl + JobPath(job) + "/config.xml", content, ctx));

        response.EnsureSuccessStatusCode();
    }

    public async Task EnableJob(string job, CancellationToken ctx = default(CancellationToken))
    {
        var content = new StringContent("");

        var response = await GetRetryPolicy().ExecuteAsync(() => httpClient.PostAsync(jenkinsConfig.JenkinsUrl + JobPath(job) + "/enable", content, ctx));

        response = await FollowRedirect(response, ctx);

        response.EnsureSuccessStatusCode();
    }

    public async Task DisableJob(string job, CancellationToken ctx = default(CancellationToken))
    {
        var content = new StringContent("");

        var response = await GetRetryPolicy().ExecuteAsync(() => httpClient.PostAsync(jenkinsConfig.JenkinsUrl + JobPath(job) + "/disable", content, ctx));

        response = await FollowRedirect(response, ctx);

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteJob(string job, CancellationToken ctx = default(CancellationToken))
    {
        var content = new StringContent("");

        var response = await GetRetryPolicy().ExecuteAsync(() => httpClient.PostAsync(jenkinsConfig.JenkinsUrl + JobPath(job) + "/doDelete", content, ctx));

        response = await FollowRedirect(response, ctx);

        response.EnsureSuccessStatusCode();
    }
        
    public async Task CreateJob(string job, string xml, CancellationToken ctx = default(CancellationToken))
    {
        await CreateJob(job, xml, "", ctx);
    }

    public async Task CreateJob(string job, string xml, string path, CancellationToken ctx = default(CancellationToken))
    {
        var content = new StringContent(xml, Encoding.UTF8, "application/xml");
        var response = await GetRetryPolicy().ExecuteAsync(() => httpClient.PostAsync(jenkinsConfig.JenkinsUrl + JobPath(path) + "/createItem?name=" + job, content, ctx));
        response = await FollowRedirect(response, ctx);
        response.EnsureSuccessStatusCode();
    }

    public async Task CreateFolder(string folder, CancellationToken ctx = default(CancellationToken))
    {
        await CreateFolder(folder, "", ctx);
    }
        
    public async Task CreateFolder(string folder, string path, CancellationToken ctx = default(CancellationToken))
    {
        var content = new StringContent("");
        var response = await GetRetryPolicy().ExecuteAsync(() => httpClient.PostAsync(jenkinsConfig.JenkinsUrl + JobPath(path) + "/createItem?name=" + folder + "&mode=com.cloudbees.hudson.plugins.folder.Folder&Submit=OK", content, ctx));
        response = await FollowRedirect(response, ctx);
        response.EnsureSuccessStatusCode();
    }
        
    public async Task DeleteFolder(string folder, CancellationToken ctx = default(CancellationToken))
    {
        //Yes, delete job/folder are the same. As this might not be transparent we have this endpoint.
        await DeleteJob(folder, ctx);
    }

    public async Task QuietDown(string reason = "", CancellationToken ctx = default(CancellationToken))
    {
        var response = await GetRetryPolicy().ExecuteAsync(() => httpClient.PostAsync(jenkinsConfig.JenkinsUrl + "/quietDown?reason=" + reason, null, ctx));
        response = await FollowRedirect(response, ctx);
        response.EnsureSuccessStatusCode();
    }
        
    public async Task CancelQuietDown(CancellationToken ctx = default(CancellationToken))
    {
        var response = await GetRetryPolicy().ExecuteAsync(() => httpClient.PostAsync(jenkinsConfig.JenkinsUrl + "/cancelQuietDown", null, ctx));
        response = await FollowRedirect(response, ctx);
        response.EnsureSuccessStatusCode();
    }
        
    public async Task Restart(CancellationToken ctx = default(CancellationToken))
    {
        await GetRetryPolicy().ExecuteAsync(() => httpClient.PostAsync(jenkinsConfig.JenkinsUrl + "/restart", null, ctx));
    }
        
    public async Task SafeRestart(CancellationToken ctx = default(CancellationToken))
    {
        await GetRetryPolicy().ExecuteAsync(() => httpClient.PostAsync(jenkinsConfig.JenkinsUrl + "/safeRestart", null, ctx));
    }
*/

    public void Dispose() => _httpClient.Dispose();

}

