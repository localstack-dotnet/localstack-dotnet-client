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
        ConsoleHelper.WriteHeader();
        ConsoleHelper.WriteRule("Initialization");

        context.StartProcess("dotnet", new ProcessSettings { Arguments = "--info" });

        if (!context.IsRunningOnUnix())
        {
            return;
        }

        context.StartProcess("git", new ProcessSettings { Arguments = "config --global core.autocrlf true" });
    }
}

[TaskName("build"), IsDependentOn(typeof(InitTask))]
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
            string targetFrameworks = string.Join(',', testProj.TargetFrameworks);

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

                            string[] containers = psOutput.Split([Environment.NewLine], StringSplitOptions.None);
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

                string testFilePrefix = targetFramework.Replace('.', '-');
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
        // Display header
        ConsoleHelper.WriteRule("Package Creation");

        // If no specific package ID is provided, pack all packages
        if (string.IsNullOrEmpty(context.PackageId))
        {
            PackAllPackages(context);
        }
        else
        {
            PackSinglePackage(context, context.PackageId);
        }

        ConsoleHelper.WriteRule();
    }

    private static void PackAllPackages(BuildContext context)
    {
        foreach (string packageId in context.PackageIdProjMap.Keys)
        {
            ConsoleHelper.WriteInfo($"Creating package: {packageId}");
            PackSinglePackage(context, packageId);
        }
    }

    private static void PackSinglePackage(BuildContext context, string packageId)
    {
        // Get effective version using enhanced methods
        string effectiveVersion = GetEffectiveVersion(context, packageId);

        // Display package information
        ConsoleHelper.WritePackageInfoTable(packageId, effectiveVersion, GetTargetFrameworks(context, packageId), context.BuildConfiguration, context.PackageSource);

        // Validate inputs
        ValidatePackageInputs(context, packageId, effectiveVersion);

        // Handle Extensions project dependency switching if needed
        if (packageId == BuildContext.LocalStackClientExtensionsProjName &&
            context is
            {
                PackageSource: BuildContext.GitHubPackageSource,
                UseDirectoryPropsVersion: false,
            })
        {
            PrepareExtensionsProject(context, effectiveVersion);
        }

        // Create packages with progress indication
        ConsoleHelper.WithProgress($"Creating {packageId} package", _ => CreatePackage(context, packageId, effectiveVersion));

        // Success message
        ConsoleHelper.WriteSuccess($"Successfully created {packageId} v{effectiveVersion}");
        ConsoleHelper.WriteInfo($"Package location: {context.ArtifactOutput}");
    }

    private static string GetEffectiveVersion(BuildContext context, string packageId)
    {
        return packageId switch
        {
            BuildContext.LocalStackClientProjName => context.GetProjectVersion(),
            BuildContext.LocalStackClientExtensionsProjName => context.GetExtensionProjectVersion(),
            _ => throw new ArgumentException($"Unknown package ID: {packageId}", nameof(packageId)),
        };
    }

    private static string GetTargetFrameworks(BuildContext context, string packageId)
    {
        // Use the existing proven method to get actual target frameworks
        return context.GetPackageTargetFrameworks(packageId);
    }

    private static void PrepareExtensionsProject(BuildContext context, string version)
    {
        ConsoleHelper.WriteProcessing("Updating Extensions project dependencies...");

        try
        {
            // Set working directory to Extensions project
            var originalWorkingDir = context.Environment.WorkingDirectory;
            context.Environment.WorkingDirectory = context.LocalStackClientExtFolder;

            try
            {
                // Remove project reference using Cake built-in method
                var projectRef = context.File("../LocalStack.Client/LocalStack.Client.csproj");
                context.DotNetRemoveReference([projectRef]);
                ConsoleHelper.WriteInfo("Removed project reference to LocalStack.Client");

                // Add package reference using Cake built-in method
                context.DotNetAddPackage(BuildContext.LocalStackClientProjName, version);
                ConsoleHelper.WriteSuccess($"Added package reference for {BuildContext.LocalStackClientProjName} v{version}");
            }
            finally
            {
                // Restore original working directory
                context.Environment.WorkingDirectory = originalWorkingDir;
            }
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteError($"Failed to prepare Extensions project: {ex.Message}");

            throw;
        }
    }

    private static void CreatePackage(BuildContext context, string packageId, string effectiveVersion)
    {
        if (!Directory.Exists(context.ArtifactOutput))
        {
            Directory.CreateDirectory(context.ArtifactOutput);
        }

        if (!context.PackageIdProjMap.TryGetValue(packageId, out FilePath? packageCsProj) || packageCsProj == null)
        {
            throw new ArgumentException($"Unknown package ID: {packageId}", nameof(packageId));
        }

        var settings = new DotNetPackSettings
        {
            Configuration = context.BuildConfiguration,
            OutputDirectory = context.ArtifactOutput,
            NoBuild = false,
            NoRestore = false,
            MSBuildSettings = new DotNetMSBuildSettings(),
        };

        settings.MSBuildSettings.SetVersion(effectiveVersion);

        context.DotNetPack(packageCsProj.FullPath, settings);
    }

    private static void ValidatePackageInputs(BuildContext context, string packageId, string effectiveVersion)
    {
        BuildContext.ValidateArgument("package-id", packageId);
        BuildContext.ValidateArgument("package-source", context.PackageSource);

        // Skip detailed version validation when using directory props version
        if (context.UseDirectoryPropsVersion)
        {
            ConsoleHelper.WriteInfo("Using dynamic version generation from Directory.Build.props");

            return;
        }

        // Original validation for manual version input
        ValidatePackageVersion(context, effectiveVersion);
    }

    private static void ValidatePackageVersion(BuildContext context, string version)
    {
        Match match = Regex.Match(version, @"^(\d+)\.(\d+)\.(\d+)([\.\-].*)*$", RegexOptions.IgnoreCase);

        if (!match.Success)
        {
            throw new Exception($"Invalid version: {version}");
        }

        // Skip version validation for GitHub Packages - allows overwriting dev builds
        if (context.PackageSource == BuildContext.GitHubPackageSource)
        {
            ConsoleHelper.WriteInfo("Skipping version validation for GitHub Packages source");

            return;
        }

        try
        {
            string packageSource = context.PackageSourceMap[context.PackageSource];
            var nuGetListSettings = new NuGetListSettings { AllVersions = false, Source = [packageSource] };
            NuGetListItem nuGetListItem = context.NuGetList(context.PackageId, nuGetListSettings).Single(item => item.Name == context.PackageId);
            string latestPackVersionStr = nuGetListItem.Version;

            Version packageVersion = Version.Parse(version);
            Version latestPackVersion = Version.Parse(latestPackVersionStr);

            if (packageVersion <= latestPackVersion)
            {
                throw new Exception($"The new package version {version} should be greater than the latest package version {latestPackVersionStr}");
            }

            ConsoleHelper.WriteSuccess($"Version validation passed: {version} > {latestPackVersionStr}");
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            ConsoleHelper.WriteWarning($"Could not validate version against existing packages: {ex.Message}");
        }
    }
}

[TaskName("nuget-push")]
public sealed class NugetPushTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        // Display header
        ConsoleHelper.WriteRule("Package Publishing");

        // Get effective version using enhanced methods
        string effectiveVersion = GetEffectiveVersion(context);

        // Validate inputs
        ValidatePublishInputs(context, effectiveVersion);

        // Display package information
        ConsoleHelper.WritePackageInfoTable(context.PackageId, effectiveVersion, GetTargetFrameworks(context), context.BuildConfiguration, context.PackageSource);

        // Perform publishing with progress indication
        ConsoleHelper.WithProgress("Publishing package", progressCtx =>
        {
            PublishPackage(context, effectiveVersion);
        });

        // Success message with download URL
        var downloadUrl = ConsoleHelper.GetDownloadUrl(context.PackageSource, context.PackageId, effectiveVersion);
        ConsoleHelper.WritePublicationSummary(context.PackageId, effectiveVersion, context.PackageSource, downloadUrl);
        ConsoleHelper.WriteRule();
    }

    private static string GetEffectiveVersion(BuildContext context)
    {
        return context.PackageId switch
        {
            BuildContext.LocalStackClientProjName => context.GetProjectVersion(),
            BuildContext.LocalStackClientExtensionsProjName => context.GetExtensionProjectVersion(),
            _ => throw new ArgumentException($"Unknown package ID: {context.PackageId}", nameof(context)),
        };
    }

    private static string GetTargetFrameworks(BuildContext context)
    {
        return context.GetPackageTargetFrameworks(context.PackageId);
    }

    private static void ValidatePublishInputs(BuildContext context, string effectiveVersion)
    {
        BuildContext.ValidateArgument("package-id", context.PackageId);
        BuildContext.ValidateArgument("package-secret", context.PackageSecret);
        BuildContext.ValidateArgument("package-source", context.PackageSource);

        // For dynamic version generation, validate the effective version instead of PackageVersion
        if (context.UseDirectoryPropsVersion)
        {
            ConsoleHelper.WriteInfo($"Using dynamic version: {effectiveVersion}");
        }
        else
        {
            BuildContext.ValidateArgument("package-version", context.PackageVersion);
        }
    }

    private static void PublishPackage(BuildContext context, string effectiveVersion)
    {
        // Use the effective version for both dynamic and manual version scenarios
        string packageVersion = context.UseDirectoryPropsVersion ? effectiveVersion : context.PackageVersion;

        ConvertableFilePath packageFile = context.ArtifactOutput + context.File($"{context.PackageId}.{packageVersion}.nupkg");

        if (!context.FileExists(packageFile))
        {
            throw new Exception($"The specified {packageFile.Path} package file does not exist");
        }

        string packageSecret = context.PackageSecret;
        string packageSource = context.PackageSourceMap[context.PackageSource];

        ConsoleHelper.WriteUpload($"Publishing {context.PackageId} to {context.PackageSource}...");

        context.DotNetNuGetPush(packageFile.Path.FullPath, new DotNetNuGetPushSettings() { ApiKey = packageSecret, Source = packageSource, });

        ConsoleHelper.WriteSuccess($"Successfully published {context.PackageId} v{packageVersion}");
    }
}