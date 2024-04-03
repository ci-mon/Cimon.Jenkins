namespace Cimon.Jenkins.WorkflowApi;

public class JenkinsWfApi
{

	public record DescribeBuild(string Build, JobLocator Job) : BaseWfApiQuery<WorkflowInfo>
	{
		public override string GetPath() => $"{Job}/{Build}";
	}

	public record GetFlowNodeLog : BaseWfApiQuery<WfConsoleLog>
	{
		private readonly string _href;
		private readonly bool _addSuffix;
		public GetFlowNodeLog(string href) {
			_href = href;
		}

		public GetFlowNodeLog(JobLocator jobLocator, long buildId, int flowNodeId) {
			_href = $"{jobLocator}/{buildId}/execution/node/{flowNodeId}/wfapi/log";
			_addSuffix = false;
		}

		public override string? ApiSuffix => _addSuffix ? base.ApiSuffix : null;

		public override string GetPath() => _href;
	}
}