using Cimon.Jenkins.Entities.Jobs;
using Cimon.Jenkins.Entities.Views;

namespace Cimon.Jenkins;

public static class Query
{
	public static BaseQuery<bool> Exists<T>(this IQuery<T> query) => new ExistsBaseQuery<T>(query);
	public record UserInfo(string UserName) : BaseQuery<Entities.Users.UserInfo>
	{
		public override string GetPath() => "/user/" + UserName;
	}

	public record Master() : BaseQuery<Entities.Master>
	{
		public override string GetPath() => string.Empty;
	}
	public record Job(JobLocator JobLocator): BaseQuery<JobInfo>
	{
		public override string GetPath() => $"{JobLocator}";
	}
	public record BuildInfo(string Build, JobLocator Job) : BaseQuery<Entities.Builds.BuildInfo>
	{
		public override string GetPath() => $"{Job}/{Build}";
	}
	public record TestsReport(BuildInfo BuildInfo) : BaseQuery<Entities.Builds.TestReport>
	{
		public override string GetPath() => $"{BuildInfo.GetPath()}/testReport";
	}
	public record BuildConsole(BuildInfo BuildInfo) : StringBaseQuery
	{
		public override string GetPath() => BuildInfo.GetPath() + "/consoleText";
	}

	public record View(string Name) : BaseQuery<ViewInfo>
	{
		public override string GetPath() => $"view/{Name}";
	}

	public record DownloadJobConfig(JobLocator Job) : StringBaseQuery
	{
		public override string GetPath() => $"{Job}/config.xml";
	}
}
