#pragma warning disable CA1515 // Consider making public types internal

namespace LocalStack.Build.CakeTasks.Nuget.Services;

/// <summary>
/// Provides high-level package operations shared across NuGet tasks.
/// Two simple methods that handle all the complexity internally.
/// </summary>
public static class PackageOperations
{
    /// <summary>
    /// Complete pack operation for a single package - handles everything from validation to success message.
    /// </summary>
    /// <param name="context">The build context</param>
    /// <param name="packageId">The package identifier</param>
    public static void PackSinglePackage(BuildContext context, string packageId)
    {
        string effectiveVersion = context.GetEffectivePackageVersion(packageId);
        string packageTargetFrameworks = context.GetPackageTargetFrameworks(packageId);

        // Display package info
        ConsoleHelper.WritePackageInfoTable(packageId, effectiveVersion, packageTargetFrameworks, context.BuildConfiguration, context.PackageSource);

        // Validate inputs
        ValidatePackInputs(context, packageId, effectiveVersion);

        // Create package with progress
        ConsoleHelper.WithProgress($"Creating {packageId} package", _ => CreatePackage(context, packageId, effectiveVersion));

        // Success message
        ConsoleHelper.WriteSuccess($"Successfully created {packageId} v{effectiveVersion}");
        ConsoleHelper.WriteInfo($"Package location: {context.ArtifactOutput}");

        // Output version to GitHub Actions if this is LocalStack.Client
        if (packageId == BuildContext.LocalStackClientProjName)
        {
            OutputVersionToGitHubActions(effectiveVersion);
        }
    }

    /// <summary>
    /// Complete publish operation for a single package - handles everything from validation to success message.
    /// </summary>
    /// <param name="context">The build context</param>
    /// <param name="packageId">The package identifier</param>
    public static void PublishSinglePackage(BuildContext context, string packageId)
    {
        string effectiveVersion = context.GetEffectivePackageVersion(packageId);
        string packageTargetFrameworks = context.GetPackageTargetFrameworks(packageId);

        // Validate inputs for publishing
        ValidatePublishInputs(context, packageId);

        // Show version info
        if (context.UseDirectoryPropsVersion)
        {
            ConsoleHelper.WriteInfo($"Using dynamic version: {effectiveVersion}");
        }
        else
        {
            ConsoleHelper.WriteInfo($"Using version: {effectiveVersion}");
        }

        // Display package info
        ConsoleHelper.WritePackageInfoTable(packageId, effectiveVersion, packageTargetFrameworks, context.BuildConfiguration, context.PackageSource);

        // Publish package with progress
        ConsoleHelper.WithProgress("Publishing package", _ => PublishPackage(context, packageId, effectiveVersion));

        // Success summary
        var downloadUrl = GetDownloadUrl(context.PackageSource, packageId, effectiveVersion);
        ConsoleHelper.WritePublicationSummary(packageId, effectiveVersion, context.PackageSource, downloadUrl);
    }

    #region Private Implementation Details

    private static void CreatePackage(BuildContext context, string packageId, string version)
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

        settings.MSBuildSettings.SetVersion(version);
        context.DotNetPack(packageCsProj.FullPath, settings);
    }

    private static void PublishPackage(BuildContext context, string packageId, string version)
    {
        ConvertableFilePath packageFile = context.ArtifactOutput + context.File($"{packageId}.{version}.nupkg");

        if (!context.FileExists(packageFile))
        {
            throw new Exception($"The specified {packageFile.Path} package file does not exist");
        }

        string packageSecret = context.PackageSecret;
        string packageSource = context.PackageSourceMap[context.PackageSource];

        ConsoleHelper.WriteUpload($"Publishing {packageId} to {context.PackageSource}...");

        context.DotNetNuGetPush(packageFile.Path.FullPath, new DotNetNuGetPushSettings()
        {
            ApiKey = packageSecret,
            Source = packageSource,
        });

        ConsoleHelper.WriteSuccess($"Successfully published {packageId} v{version}");
    }

    private static void ValidatePackInputs(BuildContext context, string packageId, string effectiveVersion)
    {
        BuildContext.ValidateArgument("package-id", packageId);
        BuildContext.ValidateArgument("package-source", context.PackageSource);

        if (context.UseDirectoryPropsVersion)
        {
            ConsoleHelper.WriteInfo("Using dynamic version generation from Directory.Build.props");
            return;
        }

        ValidatePackageVersion(context, packageId, effectiveVersion);
    }

    private static void ValidatePublishInputs(BuildContext context, string packageId)
    {
        BuildContext.ValidateArgument("package-id", packageId);
        BuildContext.ValidateArgument("package-secret", context.PackageSecret);
        BuildContext.ValidateArgument("package-source", context.PackageSource);

        if (!context.UseDirectoryPropsVersion)
        {
            BuildContext.ValidateArgument("package-version", context.PackageVersion);
        }
    }

    private static void ValidatePackageVersion(BuildContext context, string packageId, string version)
    {
        Match match = Regex.Match(version, @"^(\d+)\.(\d+)\.(\d+)([\.\-].*)*$", RegexOptions.IgnoreCase);

        if (!match.Success)
        {
            throw new Exception($"Invalid version: {version}");
        }

        if (context.PackageSource == BuildContext.GitHubPackageSource)
        {
            ConsoleHelper.WriteInfo("Skipping version validation for GitHub Packages source");
            return;
        }

        try
        {
            string packageSource = context.PackageSourceMap[context.PackageSource];
            var nuGetListSettings = new NuGetListSettings { AllVersions = false, Source = [packageSource] };
            NuGetListItem nuGetListItem = context.NuGetList(packageId, nuGetListSettings).Single(item => item.Name == packageId);
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

    private static string GetDownloadUrl(string packageSource, string packageId, string version)
    {
        return packageSource switch
        {
            BuildContext.GitHubPackageSource => $"https://github.com/localstack-dotnet/localstack-dotnet-client/packages/nuget/{packageId}",
            BuildContext.NuGetPackageSource => $"https://www.nuget.org/packages/{packageId}/{version}",
            BuildContext.MyGetPackageSource => $"https://www.myget.org/packages/{packageId}/{version}",
            _ => "Unknown package source"
        };
    }

    /// <summary>
    /// Outputs the package version to GitHub Actions for use in subsequent steps.
    /// Only outputs if running in GitHub Actions environment.
    /// </summary>
    /// <param name="version">The package version to output</param>
    private static void OutputVersionToGitHubActions(string version)
    {
        string? githubOutput = Environment.GetEnvironmentVariable("GITHUB_OUTPUT");

        if (string.IsNullOrWhiteSpace(githubOutput))
        {
            return;
        }

        try
        {
            var outputLine = $"client-version={version}";
            File.AppendAllText(githubOutput, outputLine + Environment.NewLine);
            ConsoleHelper.WriteInfo($"📤 GitHub Actions Output: {outputLine}");
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteWarning($"Failed to write to GitHub Actions output: {ex.Message}");
        }
    }

    #endregion
}