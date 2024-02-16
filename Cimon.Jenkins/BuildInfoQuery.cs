using Cimon.Jenkins.Entities.Builds;

namespace Cimon.Jenkins;

public record BuildInfoQuery(string Build, JobLocator Job) : Query<BuildInfo>
{
	public override string GetPath() {
		return $"{Job}/{Build}";
	}
}