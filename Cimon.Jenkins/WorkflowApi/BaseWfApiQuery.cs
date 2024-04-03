namespace Cimon.Jenkins.WorkflowApi;

public abstract record BaseWfApiQuery<T> : Query<T>
{
	public override string? ApiSuffix => "/wfapi/";
}