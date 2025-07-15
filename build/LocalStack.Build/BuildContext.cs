#pragma warning disable CA1515 // Consider making public types internal

namespace LocalStack.Build;

public sealed class BuildContext : FrostingContext
{
    public BuildContext(ICakeContext context) : base(context)
    {
        BuildConfiguration = context.Argument("config", "Release");
        ForceBuild = context.Argument("force-build", false);
        ForceRestore = context.Argument("force-restore", false);
        PackageVersion = context.Argument("package-version", "x.x.x");
        PackageId = context.Argument("package-id", default(string));
        PackageSecret = context.Argument("package-secret", default(string));
        PackageSource = context.Argument("package-source", default(string));
        SkipFunctionalTest = context.Argument("skipFunctionalTest", true);

        var sourceBuilder = ImmutableDictionary.CreateBuilder<string, string>();
        sourceBuilder.AddRange(new[]
        {
            new KeyValuePair<string, string>("myget", "https://www.myget.org/F/localstack-dotnet-client/api/v3/index.json"),
            new KeyValuePair<string, string>("nuget", "https://api.nuget.org/v3/index.json"),
            new KeyValuePair<string, string>("github", "https://nuget.pkg.github.com/localstack-dotnet/index.json"),
        });
        PackageSourceMap = sourceBuilder.ToImmutable();

        SolutionRoot = context.Directory("../../");
        SrcPath = SolutionRoot + context.Directory("src");
        TestsPath = SolutionRoot + context.Directory("tests");
        BuildPath = SolutionRoot + context.Directory("build");
        ArtifactOutput = SolutionRoot + context.Directory("artifacts");
        LocalStackClientFolder = SrcPath + context.Directory("LocalStack.Client");
        LocalStackClientExtFolder = SrcPath + context.Directory("LocalStack.Client.Extensions");
        SlnFilePath = SolutionRoot + context.File("LocalStack.sln");
        LocalStackClientProjFile = LocalStackClientFolder + context.File("LocalStack.Client.csproj");
        LocalStackClientExtProjFile = LocalStackClientExtFolder + context.File("LocalStack.Client.Extensions.csproj");

        var packIdBuilder = ImmutableDictionary.CreateBuilder<string, FilePath>();
        packIdBuilder.AddRange(new[]
        {
            new KeyValuePair<string, FilePath>("LocalStack.Client", LocalStackClientProjFile),
            new KeyValuePair<string, FilePath>("LocalStack.Client.Extensions", LocalStackClientExtProjFile),
        });
        PackageIdProjMap = packIdBuilder.ToImmutable();
    }

    public string BuildConfiguration { get; }

    public bool ForceBuild { get; }

    public bool ForceRestore { get; }

    public bool SkipFunctionalTest { get; }

    public string PackageVersion { get; }

    public string PackageId { get; }

    public string PackageSecret { get; }

    public string PackageSource { get; }

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

    public static void ValidateArgument(string argumentName, string argument)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            throw new Exception($"{argumentName} can not be null or empty");
        }
    }

    public void InstallXUnitNugetPackage()
    {
        if (!Directory.Exists("testrunner"))
        {
            Directory.CreateDirectory("testrunner");
        }

        var nugetInstallSettings = new NuGetInstallSettings
        {
            Version = "2.8.1", Verbosity = NuGetVerbosity.Normal, OutputDirectory = "testrunner", WorkingDirectory = ".",
        };

        this.NuGetInstall("xunit.runner.console", nugetInstallSettings);
    }

    public IEnumerable<ProjMetadata> GetProjMetadata()
    {
        DirectoryPath testsRoot = this.Directory(TestsPath);
        List<FilePath> csProjFile = this.GetFiles($"{testsRoot}/**/*.csproj")
                                        .Where(fp => fp.FullPath.EndsWith("Tests.csproj", StringComparison.InvariantCulture))
                                        .ToList();

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
        this.Information("Installing Mono on Linux for .NET Framework test platform support...");

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

    public void RunXUnitUsingMono(string targetFramework, string assemblyPath)
    {
        int exitCode = this.StartProcess(
            "mono", new ProcessSettings { Arguments = $"./testrunner/xunit.runner.console.2.8.1/tools/{targetFramework}/xunit.console.exe {assemblyPath}" });

        if (exitCode != 0)
        {
            throw new InvalidOperationException($"Exit code: {exitCode}");
        }
    }

    public string GetProjectVersion()
    {
        FilePath file = this.File("./src/Directory.Build.props");

        this.Information(file.FullPath);

        string project = File.ReadAllText(file.FullPath, Encoding.UTF8);
        int startIndex = project.IndexOf("<Version>", StringComparison.Ordinal) + "<Version>".Length;
        int endIndex = project.IndexOf("</Version>", startIndex, StringComparison.Ordinal);

        string version = project.Substring(startIndex, endIndex - startIndex);
        version = $"{version}.{PackageVersion}";

        return version;
    }

    public string GetExtensionProjectVersion()
    {
        FilePath file = this.File(LocalStackClientExtProjFile);

        this.Information(file.FullPath);

        string project = File.ReadAllText(file.FullPath, Encoding.UTF8);
        int startIndex = project.IndexOf("<Version>", StringComparison.Ordinal) + "<Version>".Length;
        int endIndex = project.IndexOf("</Version>", startIndex, StringComparison.Ordinal);

        string version = project.Substring(startIndex, endIndex - startIndex);
        version = $"{version}.{PackageVersion}";

        return version;
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

        string targetFrameworks = project.Substring(startIndex, endIndex - startIndex);

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

            assemblyName = project.Substring(startIndex, endIndex - startIndex);
        }
        else
        {
            int startIndex = csprojPath.LastIndexOf('/') + 1;
            int endIndex = csprojPath.IndexOf(".csproj", startIndex, StringComparison.Ordinal);

            assemblyName = csprojPath.Substring(startIndex, endIndex - startIndex);
        }

        return assemblyName;
    }
}