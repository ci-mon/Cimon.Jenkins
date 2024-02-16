using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Cimon.Jenkins;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add the Jenkins client to the service collection as a singleton.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The passed service collection.</returns>
    public static IServiceCollection AddJenkinsClientFactory(this IServiceCollection services) {
        services.AddHttpClient<JenkinsClient>().AddPolicyHandler(JenkinsClient.GetRetryPolicy());
        return services.AddSingleton<Func<JenkinsConfig, IJenkinsClient>>(sp => config =>
            new JenkinsClient(config, sp.GetRequiredService<IHttpClientFactory>()));
    }
}
