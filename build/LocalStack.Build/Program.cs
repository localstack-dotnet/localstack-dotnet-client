#pragma warning disable CA1515 // Consider making public types internal

return new CakeHost().UseContext<BuildContext>().Run(args);

[TaskName("Default"), IsDependentOn(typeof(TestTask))]
public class DefaultTask : FrostingTask
{
}

[TaskName("init")]
public sealed class InitTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.StartProcess("dotnet", new ProcessSettings { Arguments = "--info" });

        if (!context.IsRunningOnUnix())
        {
            return;
        }

        context.StartProcess("git", new ProcessSettings { Arguments = "config --global core.autocrlf true" });
    }
}

[TaskName("build"), IsDependentOn(typeof(InitTask)),]
public sealed class BuildTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetBuild(context.SlnFilePath, new DotNetBuildSettings { Configuration = context.BuildConfiguration });
    }
}

[TaskName("tests"), IsDependentOn(typeof(BuildTask))]
public sealed class TestTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        const string testResults = "results.trx";

        var settings = new DotNetTestSettings
        {
            NoRestore = !context.ForceRestore, NoBuild = !context.ForceBuild, Configuration = context.BuildConfiguration, Blame = true,
        };

        IEnumerable<ProjMetadata> projMetadata = context.GetProjMetadata();

        foreach (ProjMetadata testProj in projMetadata)
        {
            string testProjectPath = testProj.CsProjPath;
            string targetFrameworks = string.Join(",", testProj.TargetFrameworks);

            context.Warning($"Target Frameworks {targetFrameworks}");

            foreach (string targetFramework in testProj.TargetFrameworks)
            {
                if (context.SkipFunctionalTest && testProj.AssemblyName == "LocalStack.Client.Functional.Tests")
                {
                    context.Warning("Skipping Functional Tests");

                    continue;
                }

                context.Warning(
                    $"=============Running {targetFramework.ToUpper(System.Globalization.CultureInfo.CurrentCulture)} tests for {testProj.AssemblyName}=============");
                settings.Framework = targetFramework;

                if (testProj.AssemblyName == "LocalStack.Client.Functional.Tests")
                {
                    context.Warning("Deleting running docker containers");

                    try
                    {
                        string psOutput = context.DockerPs(new DockerContainerPsSettings() { All = true, Quiet = true });

                        if (!string.IsNullOrEmpty(psOutput))
                        {
                            context.Warning(psOutput);

                            string[] containers = psOutput.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                            context.DockerRm(containers);
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }

                // .NET Framework testing on non-Windows platforms
                // - Modern .NET includes built-in Mono runtime
                // - Test platform still requires external Mono installation on Linux
                if (targetFramework == "net472" && !context.IsRunningOnWindows())
                {
                    string platform = context.IsRunningOnLinux() ? "Linux (with external Mono)" : "macOS (built-in Mono)";
                    context.Information($"Running .NET Framework tests on {platform}");
                }

                string testFilePrefix = targetFramework.Replace(".", "-");
                settings.ArgumentCustomization = args => args.Append($" --logger \"trx;LogFileName={testFilePrefix}_{testResults}\"");
                context.DotNetTest(testProjectPath, settings);

                context.Warning("==============================================================");
            }
        }
    }
}

[TaskName("nuget-pack")]
public sealed class NugetPackTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        ValidatePackageVersion(context);

        if (!Directory.Exists(context.ArtifactOutput))
        {
            Directory.CreateDirectory(context.ArtifactOutput);
        }

        FilePath packageCsProj = context.PackageIdProjMap[context.PackageId];

        var settings = new DotNetPackSettings
        {
            Configuration = context.BuildConfiguration, OutputDirectory = context.ArtifactOutput, MSBuildSettings = new DotNetMSBuildSettings(),
        };

        settings.MSBuildSettings.SetVersion(context.PackageVersion);

        context.DotNetPack(packageCsProj.FullPath, settings);
    }

    private static void ValidatePackageVersion(BuildContext context)
    {
        BuildContext.ValidateArgument("package-id", context.PackageId);
        BuildContext.ValidateArgument("package-version", context.PackageVersion);
        BuildContext.ValidateArgument("package-source", context.PackageSource);

        Match match = Regex.Match(context.PackageVersion, @"^(\d+)\.(\d+)\.(\d+)([\.\-].*)*$", RegexOptions.IgnoreCase);

        if (!match.Success)
        {
            throw new Exception($"Invalid version: {context.PackageVersion}");
        }

        // Skip version validation for GitHub Packages - allows overwriting dev builds
        if (context.PackageSource == "github")
        {
            context.Information($"🔄 Skipping version validation for GitHub Packages source");
            return;
        }

        string packageSource = context.PackageSourceMap[context.PackageSource];

        var nuGetListSettings = new NuGetListSettings { AllVersions = false, Source = new List<string>() { packageSource } };
        NuGetListItem nuGetListItem = context.NuGetList(context.PackageId, nuGetListSettings).Single(item => item.Name == context.PackageId);
        string latestPackVersionStr = nuGetListItem.Version;

        Version packageVersion = Version.Parse(context.PackageVersion);
        Version latestPackVersion = Version.Parse(latestPackVersionStr);

        if (packageVersion <= latestPackVersion)
        {
            throw new Exception($"The new package version {context.PackageVersion} should be greater than the latest package version {latestPackVersionStr}");
        }
    }
}

[TaskName("nuget-push")]
public sealed class NugetPushTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        BuildContext.ValidateArgument("package-id", context.PackageId);
        BuildContext.ValidateArgument("package-version", context.PackageVersion);
        BuildContext.ValidateArgument("package-secret", context.PackageSecret);
        BuildContext.ValidateArgument("package-source", context.PackageSource);

        string packageId = context.PackageId;
        string packageVersion = context.PackageVersion;

        ConvertableFilePath packageFile = context.ArtifactOutput + context.File($"{packageId}.{packageVersion}.nupkg");

        if (!context.FileExists(packageFile))
        {
            throw new Exception($"The specified {packageFile.Path} package file does not exists");
        }

        string packageSecret = context.PackageSecret;
        string packageSource = context.PackageSourceMap[context.PackageSource];

        context.DotNetNuGetPush(packageFile.Path.FullPath, new DotNetNuGetPushSettings() { ApiKey = packageSecret, Source = packageSource, });
    }
}