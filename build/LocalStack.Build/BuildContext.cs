#pragma warning disable CA1515 // Consider making public types internal

namespace LocalStack.Build;

public sealed class BuildContext : FrostingContext
{
    public const string LocalStackClientProjName = "LocalStack.Client";
    public const string LocalStackClientExtensionsProjName = "LocalStack.Client.Extensions";

    public const string GitHubPackageSource = "github";
    public const string NuGetPackageSource = "nuget";
    public const string MyGetPackageSource = "myget";

    // Cached package versions to ensure consistency across pack/publish operations
    private string? _clientPackageVersion;
    private string? _extensionsPackageVersion;

    public BuildContext(ICakeContext context) : base(context)
    {
        BuildConfiguration = context.Argument("config", "Release");
        ForceBuild = context.Argument("force-build", defaultValue: false);
        ForceRestore = context.Argument("force-restore", defaultValue: false);
        PackageVersion = context.Argument("package-version", "x.x.x");
        ClientVersion = context.Argument("client-version", default(string));
        PackageId = context.Argument("package-id", default(string));
        PackageSecret = context.Argument("package-secret", default(string));
        PackageSource = context.Argument("package-source", GitHubPackageSource);
        SkipFunctionalTest = context.Argument("skipFunctionalTest", defaultValue: true);

        // New version generation arguments
        UseDirectoryPropsVersion = context.Argument("use-directory-props-version", defaultValue: false);
        BranchName = context.Argument("branch-name", "master");

        var sourceBuilder = ImmutableDictionary.CreateBuilder<string, string>();
        sourceBuilder.AddRange([
            new KeyValuePair<string, string>(MyGetPackageSource, "https://www.myget.org/F/localstack-dotnet-client/api/v3/index.json"),
            new KeyValuePair<string, string>(NuGetPackageSource, "https://api.nuget.org/v3/index.json"),
            new KeyValuePair<string, string>(GitHubPackageSource, "https://nuget.pkg.github.com/localstack-dotnet/index.json"),
        ]);
        PackageSourceMap = sourceBuilder.ToImmutable();

        SolutionRoot = context.Directory("../../");
        SrcPath = SolutionRoot + context.Directory("src");
        TestsPath = SolutionRoot + context.Directory("tests");
        BuildPath = SolutionRoot + context.Directory("build");
        ArtifactOutput = SolutionRoot + context.Directory("artifacts");
        LocalStackClientFolder = SrcPath + context.Directory(LocalStackClientProjName);
        LocalStackClientExtFolder = SrcPath + context.Directory(LocalStackClientExtensionsProjName);
        SlnFilePath = SolutionRoot + context.File("LocalStack.sln");
        LocalStackClientProjFile = LocalStackClientFolder + context.File($"{LocalStackClientProjName}.csproj");
        LocalStackClientExtProjFile = LocalStackClientExtFolder + context.File($"{LocalStackClientExtensionsProjName}.csproj");

        var packIdBuilder = ImmutableDictionary.CreateBuilder<string, FilePath>();
        packIdBuilder.AddRange(
        [
            new KeyValuePair<string, FilePath>(LocalStackClientProjName, LocalStackClientProjFile),
            new KeyValuePair<string, FilePath>(LocalStackClientExtensionsProjName, LocalStackClientExtProjFile),
        ]);
        PackageIdProjMap = packIdBuilder.ToImmutable();
    }

    public string BuildConfiguration { get; }

    public bool ForceBuild { get; }

    public bool ForceRestore { get; }

    public bool SkipFunctionalTest { get; }

    public string PackageVersion { get; }

    public string ClientVersion { get; }

    public string PackageId { get; }

    public string PackageSecret { get; }

    public string PackageSource { get; }

    public bool UseDirectoryPropsVersion { get; }

    public string BranchName { get; }

    public ImmutableDictionary<string, string> PackageSourceMap { get; }

    public ImmutableDictionary<string, FilePath> PackageIdProjMap { get; }

    public ConvertableFilePath SlnFilePath { get; }

    public ConvertableDirectoryPath SolutionRoot { get; }

    public ConvertableDirectoryPath SrcPath { get; }

    public ConvertableDirectoryPath TestsPath { get; }

    public ConvertableDirectoryPath BuildPath { get; }

    public ConvertableDirectoryPath ArtifactOutput { get; }

    public ConvertableDirectoryPath LocalStackClientFolder { get; }

    public ConvertableDirectoryPath LocalStackClientExtFolder { get; }

    public ConvertableFilePath LocalStackClientProjFile { get; }

    public ConvertableFilePath LocalStackClientExtProjFile { get; }

    /// <summary>
    /// Gets the effective package version for LocalStack.Client package.
    /// This value is cached to ensure consistency across pack and publish operations.
    /// </summary>
    public string GetClientPackageVersion()
    {
        return _clientPackageVersion ??= UseDirectoryPropsVersion
            ? GetDynamicVersionFromProps("PackageMainVersion")
            : PackageVersion;
    }

    /// <summary>
    /// Gets the effective package version for LocalStack.Client.Extensions package.
    /// This value is cached to ensure consistency across pack and publish operations.
    /// </summary>
    public string GetExtensionsPackageVersion()
    {
        return _extensionsPackageVersion ??= UseDirectoryPropsVersion
            ? GetDynamicVersionFromProps("PackageExtensionVersion")
            : PackageVersion;
    }

    /// <summary>
    /// Gets the effective package version for the specified package ID.
    /// This method provides a unified interface for accessing cached package versions.
    /// </summary>
    /// <param name="packageId">The package ID (LocalStack.Client or LocalStack.Client.Extensions)</param>
    /// <returns>The cached package version</returns>
    public string GetEffectivePackageVersion(string packageId)
    {
        return packageId switch
        {
            LocalStackClientProjName => GetClientPackageVersion(),
            LocalStackClientExtensionsProjName => GetExtensionsPackageVersion(),
            _ => throw new ArgumentException($"Unknown package ID: {packageId}", nameof(packageId)),
        };
    }

    public static void ValidateArgument(string argumentName, string argument)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            throw new Exception($"{argumentName} can not be null or empty");
        }
    }

    public IEnumerable<ProjMetadata> GetProjMetadata()
    {
        DirectoryPath testsRoot = this.Directory(TestsPath);
        List<FilePath> csProjFile = [.. this.GetFiles($"{testsRoot}/**/*.csproj").Where(fp => fp.FullPath.EndsWith("Tests.csproj", StringComparison.InvariantCulture))];

        var projMetadata = new List<ProjMetadata>();

        foreach (FilePath csProj in csProjFile)
        {
            string csProjPath = csProj.FullPath;

            IEnumerable<string> targetFrameworks = GetProjectTargetFrameworks(csProjPath);
            string directoryPath = csProj.GetDirectory().FullPath;
            string assemblyName = GetAssemblyName(csProjPath);

            var testProjMetadata = new ProjMetadata(directoryPath, csProjPath, targetFrameworks, assemblyName);
            projMetadata.Add(testProjMetadata);
        }

        return projMetadata;
    }

    public void InstallMonoOnLinux()
    {
        int result = this.StartProcess("mono", new ProcessSettings
        {
            Arguments = "--version",
            RedirectStandardOutput = true,
            NoWorkingDirectory = true,
        });

        if (result == 0)
        {
            this.Information("✅ Mono is already installed. Skipping installation.");
            return;
        }

        this.Information("Mono not found. Starting installation on Linux for .NET Framework test platform support...");

        // Add Mono repository key
        int exitCode1 = this.StartProcess("sudo", new ProcessSettings
        {
            Arguments = "apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF",
        });

        if (exitCode1 != 0)
        {
            this.Warning($"⚠️ Failed to add Mono repository key (exit code: {exitCode1})");
            return;
        }

        // Add Mono repository
        int exitCode2 = this.StartProcess("bash", new ProcessSettings
        {
            Arguments = "-c \"echo 'deb https://download.mono-project.com/repo/ubuntu focal main' | sudo tee /etc/apt/sources.list.d/mono-official-stable.list\"",
        });

        if (exitCode2 != 0)
        {
            this.Warning($"⚠️ Failed to add Mono repository (exit code: {exitCode2})");
            return;
        }

        // Update package list
        int exitCode3 = this.StartProcess("sudo", new ProcessSettings { Arguments = "apt update" });

        if (exitCode3 != 0)
        {
            this.Warning($"⚠️ Failed to update package list (exit code: {exitCode3})");
            return;
        }

        // Install Mono
        int exitCode4 = this.StartProcess("sudo", new ProcessSettings { Arguments = "apt install -y mono-complete" });

        if (exitCode4 != 0)
        {
            this.Warning($"⚠️ Failed to install Mono (exit code: {exitCode4})");
            this.Warning("This may cause .NET Framework tests to fail on Linux");
            return;
        }

        this.Information("✅ Mono installation completed successfully");
    }

    /// <summary>
    /// Gets the target frameworks for a specific package using the existing proven method
    /// </summary>
    /// <param name="packageId">The package identifier</param>
    /// <returns>Comma-separated target frameworks</returns>
    public string GetPackageTargetFrameworks(string packageId)
    {
        if (!PackageIdProjMap.TryGetValue(packageId, out FilePath? projectFile) || projectFile == null)
        {
            throw new ArgumentException($"Unknown package ID: {packageId}", nameof(packageId));
        }

        string[] frameworks = GetProjectTargetFrameworks(projectFile.FullPath);
        return string.Join(", ", frameworks);
    }

    /// <summary>
    /// Generates dynamic version from Directory.Build.props with build metadata
    /// </summary>
    /// <param name="versionPropertyName">The property name to extract (PackageMainVersion or PackageExtensionVersion)</param>
    /// <returns>Version with build metadata (e.g., 2.0.0-preview1.20240715.a1b2c3d)</returns>
    private string GetDynamicVersionFromProps(string versionPropertyName)
    {
        // Extract base version from Directory.Build.props
        FilePath propsFile = this.File("../../Directory.Build.props");
        string content = File.ReadAllText(propsFile.FullPath, Encoding.UTF8);

        string startElement = $"<{versionPropertyName}>";
        string endElement = $"</{versionPropertyName}>";

        int startIndex = content.IndexOf(startElement, StringComparison.Ordinal) + startElement.Length;
        int endIndex = content.IndexOf(endElement, startIndex, StringComparison.Ordinal);

        if (startIndex < startElement.Length || endIndex < 0)
        {
            throw new InvalidOperationException($"Could not find {versionPropertyName} in Directory.Build.props");
        }

        string baseVersion = content[startIndex..endIndex];

        // Generate build metadata
        string buildDate = DateTime.UtcNow.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
        string commitSha = GetGitCommitSha();
        string safeBranchName = BranchName.Replace('/', '-').Replace('_', '-');

        // Simplified NuGet-compliant version format
        if (BranchName == "master")
        {
            // Nightly off main — use build metadata
            return $"{baseVersion}+{buildDate}.{commitSha}";
        }
        else
        {
            // Feature branch / preview line
            return $"{baseVersion}.{safeBranchName}.{buildDate}.{commitSha}";
        }
    }

    /// <summary>
    /// Gets the short git commit SHA for version metadata
    /// </summary>
    /// <returns>Short commit SHA or timestamp fallback</returns>
    private string GetGitCommitSha()
    {
        try
        {
            var processSettings = new ProcessSettings
            {
                Arguments = "rev-parse --short HEAD",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Silent = true,
            };

            var exitCode = this.StartProcess("git", processSettings, out IEnumerable<string> output);

            if (exitCode == 0 && output?.Any() == true)
            {
                string? commitSha = output.FirstOrDefault()?.Trim();
                if (!string.IsNullOrEmpty(commitSha))
                {
                    return commitSha;
                }
            }
        }
        catch (Exception ex)
        {
            this.Warning($"Failed to get git commit SHA: {ex.Message}");
        }

        // Fallback to timestamp-based identifier
        return DateTime.UtcNow.ToString("HHmmss", System.Globalization.CultureInfo.InvariantCulture);
    }

    private string[] GetProjectTargetFrameworks(string csprojPath)
    {
        FilePath file = this.File(csprojPath);
        string project = File.ReadAllText(file.FullPath, Encoding.UTF8);

        bool multipleFrameworks = project.Contains("<TargetFrameworks>");
        string startElement = multipleFrameworks ? "<TargetFrameworks>" : "<TargetFramework>";
        string endElement = multipleFrameworks ? "</TargetFrameworks>" : "</TargetFramework>";

        int startIndex = project.IndexOf(startElement, StringComparison.Ordinal) + startElement.Length;
        int endIndex = project.IndexOf(endElement, startIndex, StringComparison.Ordinal);

        string targetFrameworks = project[startIndex..endIndex];

        return targetFrameworks.Split(';');
    }

    private string GetAssemblyName(string csprojPath)
    {
        FilePath file = this.File(csprojPath);
        string project = File.ReadAllText(file.FullPath, Encoding.UTF8);

        bool assemblyNameElementExists = project.Contains("<AssemblyName>");

        string assemblyName;

        if (assemblyNameElementExists)
        {
            int startIndex = project.IndexOf("<AssemblyName>", StringComparison.Ordinal) + "<AssemblyName>".Length;
            int endIndex = project.IndexOf("</AssemblyName>", startIndex, StringComparison.Ordinal);

            assemblyName = project[startIndex..endIndex];
        }
        else
        {
            int startIndex = csprojPath.LastIndexOf('/') + 1;
            int endIndex = csprojPath.IndexOf(".csproj", startIndex, StringComparison.Ordinal);

            assemblyName = csprojPath[startIndex..endIndex];
        }

        return assemblyName;
    }
}