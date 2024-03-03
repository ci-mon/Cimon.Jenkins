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

	private static IEnumerable<Command> CommandsCases {
		get {
			yield return new Commands.Restart();
			yield return new Commands.SafeRestart();
		}
	}

	[Test, TestCaseSource(nameof(CommandsCases))]
	public async Task Commands(Command command) {
		await _jenkinsClient.Post(command);
	}

	[Test]
	public async Task QuietDown() {
		await _jenkinsClient.Post(new Commands.CancelQuietDown());
		var master = await _jenkinsClient.Get(new Queries.Master());
		master?.QuietingDown.Should().BeFalse();
		await _jenkinsClient.Post(new Commands.QuietDown("because"));
		master = await _jenkinsClient.Get(new Queries.Master());
		master?.QuietingDown.Should().BeTrue();
		await _jenkinsClient.Post(new Commands.CancelQuietDown());
		master = await _jenkinsClient.Get(new Queries.Master());
		master?.QuietingDown.Should().BeFalse();
	}

	[Test]
	public async Task Folder() {
		await _jenkinsClient.Post(new Commands.CreateFolder("rootFolder"));
		await _jenkinsClient.Post(new Commands.CreateFolder(JobLocator.Create("rootFolder", "innerFolder")));
		var locator = JobLocator.Create("rootFolder", "innerFolder", "innerSubFolder");
		await _jenkinsClient.Post(new Commands.CreateFolder(locator));
		var folder = await _jenkinsClient.Get(new Queries.Job(locator));
		folder.Should().NotBeNull();
		await _jenkinsClient.Post(new Commands.DeleteFolder(locator));
		folder = await _jenkinsClient.Get(new Queries.Job(locator));
		folder.Should().BeNull();
		await _jenkinsClient.Post(new Commands.DeleteFolder("rootFolder"));
		folder = await _jenkinsClient.Get(new Queries.Job("rootFolder"));
		folder.Should().BeNull();
	}

	[Test]
	public async Task Commands_DisableJob() {
		var locator = JobLocator.Create("test proj");
		await _jenkinsClient.Post(new Commands.DisableJob(locator));
		var job = await _jenkinsClient.Get(new Queries.Job(locator));
		job!.Disabled.Should().BeTrue();
		await _jenkinsClient.Post(new Commands.EnableJob(locator));
		job = await _jenkinsClient.Get(new Queries.Job(locator));
		job!.Disabled.Should().BeFalse();
	}

	[Test]
	public async Task Commands_CopyJob() {
		var folder = "test_tmp_folder";
		await _jenkinsClient.Post(new Commands.CreateFolder(folder));
		var sourceJob = JobLocator.Create("test proj");
		var destJob = JobLocator.Create(folder, "test_tmp");
		await _jenkinsClient.Post(new Commands.CopyJob(destJob, sourceJob));
		var destJob2 = JobLocator.Create(folder, "test_tmp_2");
		await _jenkinsClient.Post(new Commands.CopyJob(destJob2, destJob));
		var job = await _jenkinsClient.Get(new Queries.Job(destJob));
		job!.Should().NotBeNull();
		await _jenkinsClient.Post(new Commands.DeleteFolder(folder));
	}

	[Test]
	public async Task Commands_CreateJob() {
		var folder = "test_tmp_folder_2";
		await _jenkinsClient.Post(new Commands.CreateFolder(folder));
		var sourceJob = JobLocator.Create("test proj");
		var destJob = JobLocator.Create(folder, "test_tmp");
		var sourceJobXml = await _jenkinsClient.Get(new Queries.DownloadJobConfig(sourceJob));
		await _jenkinsClient.Post(new Commands.CreateJob(destJob, _emptyConfig!));
		var job = await _jenkinsClient.Get(new Queries.Job(destJob));
		job!.Should().NotBeNull();
		await _jenkinsClient.Post(new Commands.UploadJobConfig(destJob, sourceJobXml));
		var jobXml = await _jenkinsClient.Get(new Queries.DownloadJobConfig(sourceJob));
		jobXml.Should().Be(sourceJobXml);
		await _jenkinsClient.Post(new Commands.DeleteFolder(folder));
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
		await _jenkinsClient.Post(new Commands.Build(jobLocator));
	}

	[Test]
	public async Task Commands_Build_Params() {
		var jobLocator = JobLocator.Create("test1", "master");
		await _jenkinsClient.Post(new Commands.Build(jobLocator) {
			Parameters = {
				{"PERSON", "cimon"}
			}
		});
	}

	[Test]
	public async Task Queries() {

		var userInfo = await _jenkinsClient.Get(new Queries.UserInfo("admin"));
		userInfo.Should().NotBeNull();

		var master = await _jenkinsClient.Get(new Queries.Master());
		master.Should().NotBeNull();

		var view = await _jenkinsClient.Get(master!.Views.First().ToQuery());
		view.Should().NotBeNull();

		view = await _jenkinsClient.Get(new Queries.View(master!.Views.Last().Name));
		view.Should().NotBeNull();

		var jobLocator = JobLocator.Create("test1", "master");
		var job = await _jenkinsClient.Get(new Queries.Job(jobLocator));
		job.Should().NotBeNull();

		var config = await _jenkinsClient.Get(new Queries.DownloadJobConfig(jobLocator));
		config.Should().NotBeNullOrWhiteSpace();

		(await _jenkinsClient.Get(new Queries.Job(jobLocator).Exists())).Should().BeTrue();
		(await _jenkinsClient.Get(new Queries.Job(JobLocator.Create("thisJobNotExist")).Exists())).Should().BeFalse();

		var buildInfoQuery = new Queries.BuildInfo("20", jobLocator);
		var buildInfo = await _jenkinsClient.Get(buildInfoQuery);
		buildInfo.Should().NotBeNull();

		var testReport = await _jenkinsClient.Get(new Queries.TestsReport(buildInfoQuery));
		testReport.Should().NotBeNull();

		var console = await _jenkinsClient.Get(new Queries.BuildConsole(buildInfoQuery));
		console.Should().NotBeNullOrEmpty();

	}

}
