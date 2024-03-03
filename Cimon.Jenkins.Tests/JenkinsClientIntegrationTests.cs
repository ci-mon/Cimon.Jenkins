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
			ApiKey = "11bfe3083afe1b8ef8f7cef87ac4d847ec"
		});
	}

	[TearDown]
	protected void TearDown() {
		_jenkinsClient.Dispose();
	}

	[Test]
	public async Task Restart() {
		await _jenkinsClient.Exec(new API.Restart());
	}

	[Test]
	public async Task SafeRestart() {
		await _jenkinsClient.Exec(new API.SafeRestart());
	}

	[Test]
	public async Task QuietDown() {
		await _jenkinsClient.Exec(new API.CancelQuietDown());
		var master = await _jenkinsClient.Query(new API.Master());
		master?.QuietingDown.Should().BeFalse();
		await _jenkinsClient.Exec(new API.QuietDown("because"));
		master = await _jenkinsClient.Query(new API.Master());
		master?.QuietingDown.Should().BeTrue();
		await _jenkinsClient.Exec(new API.CancelQuietDown());
		master = await _jenkinsClient.Query(new API.Master());
		master?.QuietingDown.Should().BeFalse();
	}

	[Test]
	public async Task Folder() {
		await _jenkinsClient.Exec(new API.CreateFolder("rootFolder"));
		await _jenkinsClient.Exec(new API.CreateFolder(JobLocator.Create("rootFolder", "innerFolder")));
		var locator = JobLocator.Create("rootFolder", "innerFolder", "innerSubFolder");
		await _jenkinsClient.Exec(new API.CreateFolder(locator));
		var folder = await _jenkinsClient.Query(new API.Job(locator));
		folder.Should().NotBeNull();
		await _jenkinsClient.Exec(new API.DeleteFolder(locator));
		folder = await _jenkinsClient.Query(new API.Job(locator));
		folder.Should().BeNull();
		await _jenkinsClient.Exec(new API.DeleteFolder("rootFolder"));
		folder = await _jenkinsClient.Query(new API.Job("rootFolder"));
		folder.Should().BeNull();
	}

	[Test]
	public async Task Commands_DisableJob() {
		var locator = JobLocator.Create("test proj");
		await _jenkinsClient.Exec(new API.DisableJob(locator));
		var job = await _jenkinsClient.Query(new API.Job(locator));
		job!.Disabled.Should().BeTrue();
		await _jenkinsClient.Exec(new API.EnableJob(locator));
		job = await _jenkinsClient.Query(new API.Job(locator));
		job!.Disabled.Should().BeFalse();
	}

	[Test]
	public async Task Commands_CopyJob() {
		var folder = "test_tmp_folder";
		await _jenkinsClient.Exec(new API.CreateFolder(folder));
		var sourceJob = JobLocator.Create("test proj");
		var destJob = JobLocator.Create(folder, "test_tmp");
		await _jenkinsClient.Exec(new API.CopyJob(destJob, sourceJob));
		var destJob2 = JobLocator.Create(folder, "test_tmp_2");
		await _jenkinsClient.Exec(new API.CopyJob(destJob2, destJob));
		var job = await _jenkinsClient.Query(new API.Job(destJob));
		job!.Should().NotBeNull();
		await _jenkinsClient.Exec(new API.DeleteFolder(folder));
	}

	[Test]
	public async Task Commands_CreateJob() {
		var folder = "test_tmp_folder_2";
		await _jenkinsClient.Exec(new API.CreateFolder(folder));
		var sourceJob = JobLocator.Create("test proj");
		var destJob = JobLocator.Create(folder, "test_tmp");
		var sourceJobXml = await _jenkinsClient.Query(new API.DownloadJobConfig(sourceJob));
		await _jenkinsClient.Exec(new API.CreateJob(destJob, _emptyConfig!));
		var job = await _jenkinsClient.Query(new API.Job(destJob));
		job!.Should().NotBeNull();
		await _jenkinsClient.Exec(new API.UploadJobConfig(destJob, sourceJobXml!));
		var jobXml = await _jenkinsClient.Query(new API.DownloadJobConfig(sourceJob));
		jobXml.Should().Be(sourceJobXml);
		await _jenkinsClient.Exec(new API.DeleteFolder(folder));
	}

	private readonly string _emptyConfig = """
		<?xml version="1.1" encoding="UTF-8" standalone="no"?>
		<project>
		    <actions/>
		    <description/>
		    <keepDependencies>false</keepDependencies>
		    <properties/>
		    <scm class="hudson.scm.NullSCM"/>
		    <canRoam>true</canRoam>
		    <triggers/>
		    <concurrentBuild>false</concurrentBuild>
		    <builders />
		    <publishers/>
		    <buildWrappers/>
		</project>
		""";

	[Test]
	public async Task Commands_Build() {
		var jobLocator = JobLocator.Create("test proj");
		await _jenkinsClient.Exec(new API.Build(jobLocator));
	}

	[Test]
	public async Task Commands_Build_Params() {
		var jobLocator = JobLocator.Create("test1", "master");
		await _jenkinsClient.Exec(new API.Build(jobLocator) {
			Parameters = {
				{"PERSON", "cimon"}
			}
		});
	}

	[Test]
	public async Task Queries() {

		var userInfo = await _jenkinsClient.Query(new API.UserInfo("admin"));
		userInfo.Should().NotBeNull();

		var master = await _jenkinsClient.Query(new API.Master());
		master.Should().NotBeNull();

		var view = await _jenkinsClient.Query(master!.Views.First().ToQuery());
		view.Should().NotBeNull();

		view = await _jenkinsClient.Query(new API.View(master.Views.Last().Name!));
		view.Should().NotBeNull();

		var jobLocator = JobLocator.Create("test1", "master");
		var job = await _jenkinsClient.Query(new API.Job(jobLocator));
		job.Should().NotBeNull();

		var config = await _jenkinsClient.Query(new API.DownloadJobConfig(jobLocator));
		config.Should().NotBeNullOrWhiteSpace();

		(await _jenkinsClient.Query(new API.Job(jobLocator).Exists())).Should().BeTrue();
		(await _jenkinsClient.Query(new API.Job(JobLocator.Create("thisJobNotExist")).Exists())).Should().BeFalse();

		var buildInfoQuery = new API.BuildInfo("20", jobLocator);
		var buildInfo = await _jenkinsClient.Query(buildInfoQuery);
		buildInfo.Should().NotBeNull();

		var testReport = await _jenkinsClient.Query(new API.TestsReport(buildInfoQuery));
		testReport.Should().NotBeNull();

		var console = await _jenkinsClient.Query(new API.BuildConsole(buildInfoQuery));
		console.Should().NotBeNullOrEmpty();

	}

}
