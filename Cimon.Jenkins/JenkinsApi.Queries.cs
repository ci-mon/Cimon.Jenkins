using Cimon.Jenkins.Entities.Jobs;
using Cimon.Jenkins.Entities.Views;

namespace Cimon.Jenkins;

public static partial class JenkinsApi
{
	public static Query<bool> Exists<T>(this Query<T> query) => new ExistsQuery<T>(query);
	public record UserInfo(string UserName) : Query<Entities.Users.UserInfo>
	{
		public override string GetPath() => "/user/" + UserName;
	}

	public record Master : Query<Entities.Master>
	{
		public override string GetPath() => string.Empty;
	}
	public record Job(JobLocator JobLocator): Query<JobInfo>
	{
		public override string GetPath() => $"{JobLocator}";
	}
	public record BuildInfo(string Build, JobLocator Job) : Query<Entities.Builds.BuildInfo>
	{
		public override string GetPath() => $"{Job}/{Build}";
	}
	public record TestsReport(BuildInfo BuildInfo) : Query<Entities.Builds.TestReport>
	{
		public override string GetPath() => $"{BuildInfo.GetPath()}/testReport";
	}
	public record BuildConsole(BuildInfo BuildInfo) : StringQuery
	{
		public override string GetPath() => BuildInfo.GetPath() + "/consoleText";
	}

	public record View(string Name) : Query<ViewInfo>
	{
		public override string GetPath() => $"view/{Name}";
	}

	public record DownloadJobConfig(JobLocator Job) : StringQuery
	{
		public override string GetPath() => $"{Job}/config.xml";
	}
}
