using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

var target = Argument("target", "default");
var configuration = Argument("config", "Release");
var buildNumber = Argument<int>("buildnumber", 1);

var artifactOutput = "./artifacts";
var testResults = "results.trx";
string projectPath = "./src/LocalStack.Client/LocalStack.Client.csproj";

Task("default")
    .IsDependentOn("init")
    .IsDependentOn("tests");

Task("init")
    .Description("Initialize task prerequisites")
    .Does(() =>
    {
        StartProcess("dotnet", new ProcessSettings {
            Arguments = "--info"
        });

        if(IsRunningOnUnix())
        {
            StartProcess("git", new ProcessSettings {
                Arguments = "config --global core.autocrlf true"
            });

            StartProcess("mono", new ProcessSettings {
                Arguments = "--info"
            });

            InstallXUnitNugetPackage();
        }      
    });

Task("compile")
    .Description("Builds all the projects in the solution")
    .Does(() =>
    {
        string slnPath = "./src/LocalStack.sln";

        DotNetCoreBuildSettings settings = new DotNetCoreBuildSettings();
        settings.Configuration = configuration;
        DotNetCoreBuild(slnPath, settings);
    });

Task("tests")
    .Description("Run Tests")
    .IsDependentOn("compile")
    .Does(() =>
    {      
        DotNetCoreTestSettings settings = new DotNetCoreTestSettings();
        settings.NoRestore = true;
        settings.NoBuild = true;
        settings.Configuration = configuration;

        IList<TestProjMetadata> testProjMetadatas = GetProjMetadata();

        foreach (var testProj in testProjMetadatas)
        {
           string testProjectPath = testProj.CsProjPath;

           Warning($"Target Frameworks {string.Join(" ",testProj.TargetFrameworks)}");

           foreach(string targetFramework in testProj.TargetFrameworks)
           {
                Warning($"Running {targetFramework.ToUpper()} tests for {testProj.AssemblyName}");
                settings.Framework = targetFramework;

                if(IsRunningOnUnix() && targetFramework == "net461")
                {
                    RunXunitUsingMono(targetFramework, $"{testProj.DirectoryPath}/bin/{configuration}/{targetFramework}/{testProj.AssemblyName}.dll");
                }
                else
                {
                    string testFilePrefix = targetFramework.Replace(".","-");
                    settings.ArgumentCustomization  = args => args.Append($" --logger \"trx;LogFileName={testFilePrefix}_{testResults}\"");
                    DotNetCoreTest(testProjectPath, settings);
                }
           }
        }
    });

Task("nuget-pack")
    .Does(() =>
    {
        string outputDirectory = MakeAbsolute(Directory(artifactOutput)).FullPath;
        string projectFullPath = MakeAbsolute(File(projectPath)).FullPath;

        if(!System.IO.Directory.Exists(outputDirectory))
        {
            System.IO.Directory.CreateDirectory(outputDirectory);
        }

        var settings = new DotNetCorePackSettings();
        settings.Configuration = configuration;
        settings.OutputDirectory = artifactOutput;
        settings.MSBuildSettings = new DotNetCoreMSBuildSettings();
        settings.MSBuildSettings.SetVersion(GetProjectVersion());

        DotNetCorePack(projectFullPath, settings);
    });

Task("get-version")
    .Description("Get version")
    .Does(() =>
    {
        Warning(GetProjectVersion());
    });

RunTarget(target);

/*
/ HELPER METHODS
*/
private void InstallXUnitNugetPackage()
{
    NuGetInstallSettings nugetInstallSettings = new NuGetInstallSettings();
    nugetInstallSettings.Version = "2.4.1";
    nugetInstallSettings.Verbosity = NuGetVerbosity.Normal;
    nugetInstallSettings.OutputDirectory = "testrunner";            
    nugetInstallSettings.WorkingDirectory = ".";

    NuGetInstall("xunit.runner.console", nugetInstallSettings);
}

private void RunXunitUsingMono(string targetFramework, string assemblyPath)
{
    int exitCode = StartProcess("mono", new ProcessSettings {
        Arguments = $"./testrunner/xunit.runner.console.2.4.1/tools/{targetFramework}/xunit.console.exe {assemblyPath}"
    });

    if(exitCode != 0)
    {
        throw new InvalidOperationException($"Exit code: {exitCode}");
    }
}

private IList<TestProjMetadata> GetProjMetadata()
{
    var testsRoot = MakeAbsolute(Directory("./tests/"));
    var csProjs = GetFiles($"{testsRoot}/**/*.csproj").Where(fp => fp.FullPath.EndsWith("Tests.csproj")).ToList();

    IList<TestProjMetadata> testProjMetadatas = new List<TestProjMetadata>();

    foreach (var csProj in csProjs)
    {
        string csProjPath = csProj.FullPath;

        string[] targetFrameworks = GetProjectTargetFrameworks(csProjPath);
        string directoryPath = csProj.GetDirectory().FullPath;
        string assemblyName = GetAssemblyName(csProjPath);

        var testProjMetadata = new TestProjMetadata(directoryPath, csProjPath, targetFrameworks, assemblyName);
        testProjMetadatas.Add(testProjMetadata);
    }

    return testProjMetadatas;
}

private string[] GetProjectTargetFrameworks(string csprojPath)
{
    var file =  MakeAbsolute(File(csprojPath));
    var project = System.IO.File.ReadAllText(file.FullPath, Encoding.UTF8);

    bool multipleFrameworks = project.Contains("<TargetFrameworks>");
    string startElement = multipleFrameworks ? "<TargetFrameworks>" : "<TargetFramework>";
    string endElement = multipleFrameworks ? "</TargetFrameworks>" : "</TargetFramework>";

    int startIndex = project.IndexOf(startElement) + startElement.Length;
    int endIndex = project.IndexOf(endElement, startIndex);

    string targetFrameworks = project.Substring(startIndex, endIndex - startIndex);
    return targetFrameworks.Split(';');
}

private string GetAssemblyName(string csprojPath)
{
    var file =  MakeAbsolute(File(csprojPath));
    var project = System.IO.File.ReadAllText(file.FullPath, Encoding.UTF8);
    
    bool assemblyNameElementExists = project.Contains("<AssemblyName>");

    string assemblyName = string.Empty;

    if(assemblyNameElementExists)
    {
        int startIndex = project.IndexOf("<AssemblyName>") + "<AssemblyName>".Length;
        int endIndex = project.IndexOf("</AssemblyName>", startIndex);

        assemblyName = project.Substring(startIndex, endIndex - startIndex);
    }
    else
    {        
        int startIndex = csprojPath.LastIndexOf("/") + 1;
        int endIndex = csprojPath.IndexOf(".csproj", startIndex);

        assemblyName = csprojPath.Substring(startIndex, endIndex - startIndex);
    }

    return assemblyName;
}

private void UpdateProjectVersion(string version)
{
    Information("Setting version to " + version);

    if(string.IsNullOrWhiteSpace(version))
    {
        throw new CakeException("No version specified! You need to pass in --targetversion=\"x.y.z\"");
    }

    var file =  MakeAbsolute(File("./src/Directory.Build.props"));

    Information(file.FullPath);

    var project = System.IO.File.ReadAllText(file.FullPath, Encoding.UTF8);

    var projectVersion = new Regex(@"<Version>.+<\/Version>");
    project = projectVersion.Replace(project, string.Concat("<Version>", version, "</Version>"));

    System.IO.File.WriteAllText(file.FullPath, project, Encoding.UTF8);
}

private string GetProjectVersion()
{
    var file =  MakeAbsolute(File("./src/Directory.Build.props"));

    Information(file.FullPath);

    var project = System.IO.File.ReadAllText(file.FullPath, Encoding.UTF8);
    int startIndex = project.IndexOf("<Version>") + "<Version>".Length;
    int endIndex = project.IndexOf("</Version>", startIndex);

    string version = project.Substring(startIndex, endIndex - startIndex);
    version = $"{version}.{buildNumber}";

    return version;
}

/*
/ MODELS
*/
public class TestProjMetadata
{
   public TestProjMetadata(string directoryPath, string csProjPath, string[] targetFrameworks, string assemblyName) 
       => (DirectoryPath, CsProjPath, TargetFrameworks, AssemblyName) = (directoryPath, csProjPath, targetFrameworks, assemblyName);

   public string DirectoryPath { get; }
   public string CsProjPath { get; }
   public string AssemblyName { get; set; }
   public string[] TargetFrameworks { get; }
}