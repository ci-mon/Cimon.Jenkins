using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Cimon.Jenkins;

public static class Commands
{
	public record QuietDown(string? Reason = null) : Command
	{
		public override string GetPath() => $"/quietDown?reason={Reason}";
	}
	public record CancelQuietDown : Command
	{
		public override string GetPath() => $"/cancelQuietDown";
	}

	public record Restart : Command;
	public record SafeRestart : Command;

	public record DeleteJob(JobLocator JobLocator) : Command
	{
		public override string GetPath() => $"{JobLocator}/doDelete";
	}

	public record DeleteFolder(JobLocator JobLocator) : DeleteJob(JobLocator);

	public abstract record CommandWithEmptyContent : Command
	{

		protected override HttpContent? GetContent() => new StringContent("");
	}

	public record CreateFolder(JobLocator Locator) : CommandWithEmptyContent
	{
		public override string GetPath() {
			var (name, path) = Locator;
			return $"{path}/createItem?name={name}&mode=com.cloudbees.hudson.plugins.folder.Folder&Submit=OK";
		}

	}

	public record DisableJob(JobLocator Locator) : CommandWithEmptyContent
	{
		public override string GetPath() => $"{Locator}/disable";
	}
	public record EnableJob(JobLocator Locator) : CommandWithEmptyContent
	{
		public override string GetPath() => $"{Locator}/enable";
	}

	public record CopyJob(JobLocator Locator, JobLocator Source) : CreateJob(Locator, "")
	{
		public override string GetPath() => $"{base.GetPath()}&mode=copy&from={Source.Path}";
	}

	public record BaseJobConfigCommand(string Xml) : Command
	{
		protected override HttpContent? GetContent() => new StringContent(Xml, Encoding.UTF8, "application/xml");
	}
	public record CreateJob(JobLocator Locator, string Xml) : BaseJobConfigCommand(Xml)
	{
		public override string GetPath() {
			var (name, path) = Locator;
			return $"{path}/createItem?name={name}";
		}

	}

	public record UploadJobConfig(JobLocator Locator, string Xml) : BaseJobConfigCommand(Xml)
	{
		public override string GetPath() => $"{Locator}/config.xml";
	}

	public record Build(JobLocator Job) : Command
	{
		public override string GetPath() => $"{Job}/build";

		public Dictionary<string, object> Parameters { get; init; } = new();

		protected override HttpContent? GetContent() {
			if (Parameters.Count == 0) return null;
			var parametersArray = new {
				parameter = Parameters.Select(x => new { name = x.Key, value = x.Value }).ToArray()
			};
			var json = JsonSerializer.Serialize(parametersArray);
			return new FormUrlEncodedContent(new[] {
				new KeyValuePair<string, string>("json", json)
			});
		}
	}
}
