using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Cimon.Jenkins.Tests;

public class JenkinsClientIntegrationTests
{
	private IJenkinsClient _jenkinsClient = null!;
	[SetUp]
	public void Setup() {
		var services = new ServiceCollection().AddHttpClient().AddJenkinsClientFactory().BuildServiceProvider();
		_jenkinsClient = services.GetRequiredService<Func<JenkinsConfig, IJenkinsClient>>().Invoke(new JenkinsConfig {
			JenkinsUrl = new Uri("http://localhost:8080/"),
			Username = "admin",
			ApiKey = "admin"
		});
	}

	[TearDown]
	protected void TearDown() {
		_jenkinsClient.Dispose();
	}

	[Test]
	public async Task BuildInfoQuery() {

		var userInfo = await _jenkinsClient.Get(new Query.UserInfo("admin"));
		userInfo.Should().NotBeNull();

		var master = await _jenkinsClient.Get(new Query.Master());
		master.Should().NotBeNull();

		var view = await _jenkinsClient.Get(master!.Views.First().ToQuery());
		view.Should().NotBeNull();

		view = await _jenkinsClient.Get(new Query.View(master!.Views.Last().Name));
		view.Should().NotBeNull();

		var jobLocator = JobLocator.Create("test1", "master");
		var job = await _jenkinsClient.Get(new Query.Job(jobLocator));
		job.Should().NotBeNull();

		var config = await _jenkinsClient.Get(new Query.DownloadJobConfig(jobLocator));
		config.Should().NotBeNullOrWhiteSpace();

		(await _jenkinsClient.Get(new Query.Job(jobLocator).Exists())).Should().BeTrue();
		(await _jenkinsClient.Get(new Query.Job(JobLocator.Create("thisJobNotExist")).Exists())).Should().BeFalse();

		var buildInfoQuery = new Query.BuildInfo("10", jobLocator);
		var buildInfo = await _jenkinsClient.Get(buildInfoQuery);
		buildInfo.Should().NotBeNull();

		var testReport = await _jenkinsClient.Get(new Query.TestsReport(buildInfoQuery));
		testReport.Should().NotBeNull();

		var console = await _jenkinsClient.Get(new Query.BuildConsole(buildInfoQuery));
		console.Should().NotBeNullOrEmpty();

	}

}
