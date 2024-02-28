# Cimon.Jenkins [![NuGet](https://img.shields.io/nuget/v/Cimon.Jenkins.svg)](https://www.nuget.org/packages/Cimon.Jenkins/)
This is a fork of Narochno.Jenkins a simple Jenkins client, providing a C# wrapper around the default Jenkins API.

## Example Usage
```csharp
    using var services = new ServiceCollection().AddHttpClient().AddJenkinsClientFactory().BuildServiceProvider();
    using var client = services.GetRequiredService<Func<JenkinsConfig, IJenkinsClient>>().Invoke(new JenkinsConfig {
        JenkinsUrl = new Uri("http://localhost:8080/"),
        Username = "admin",
        ApiKey = "admin"
    });
    var userInfo = await client.Get(new Query.UserInfo("admin"));
    var master = await client.Get(new Query.Master());
    var view = await client.Get(master!.Views.First().ToQuery());
    var jobLocator = JobLocator.Create("test1", "master");
    var job = await client.Get(new Query.Job(jobLocator));
    var config = await client.Get(new Query.DownloadJobConfig(jobLocator));
    var buildInfoQuery = new Query.BuildInfo("10", jobLocator);
    var buildInfo = await client.Get(buildInfoQuery);
    var testReport = await client.Get(new Query.TestsReport(buildInfoQuery));
    
    var changedFiles = buildInfo.ChangeSets.SelectMany(x => x.Items).SelectMany(x => x.Paths).Select(x => x.File);
    var tests = testReport.Suites.SelectMany(x => x.Cases).Select(x => x.Name);
    
    var console = await client.Get(new Query.BuildConsole(buildInfoQuery));
```
