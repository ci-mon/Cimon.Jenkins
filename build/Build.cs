using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;

class Build : NukeBuild
{
	[Parameter] [Secret] readonly string NuGetApiKey;
	static readonly AbsolutePath PackagesDirectory = RootDirectory / "Packages";

	public static int Main () => Execute<Build>(x => x.Compile);

	[Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
	readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

	[Solution]
	readonly Solution Solution;

	Target Clean => _ => _
		.Executes(() => {
			PackagesDirectory.DeleteDirectory();
			DotNetTasks.DotNetClean();
		});

	Target Compile => _ => _
		.Executes(() => {
			DotNetTasks.DotNetBuild();
		});

	Target Pack => _ => _
		.DependsOn(Clean)
		.Produces(PackagesDirectory / "*.nupkg")
		.Executes(() => {
			DotNetTasks.DotNetPack(settings => settings.SetOutputDirectory(PackagesDirectory));
		});

	Target Push => _ => _
		.DependsOn(Pack)
		.Requires(() => NuGetApiKey)
		.Executes(() => {
			var packageFiles = PackagesDirectory.GlobFiles("*.nupkg");
			DotNetTasks.DotNetNuGetPush(settings =>
				settings
					.SetSource("https://api.nuget.org/v3/index.json")
					.SetApiKey(NuGetApiKey)
					.CombineWith(packageFiles, (pushSettings, path) => pushSettings.SetTargetPath(path)));
		});
}
