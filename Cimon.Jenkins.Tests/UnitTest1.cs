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
		var buildInfo = await _jenkinsClient.Get(new BuildInfoQuery("10", JobLocator.Create("test1", "master")));
		Assert.NotNull(buildInfo);
	}
}
