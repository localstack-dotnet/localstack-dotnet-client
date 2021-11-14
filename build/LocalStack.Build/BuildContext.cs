using Cake.Common;
using Cake.Common.IO;
using Cake.Common.IO.Paths;
using Cake.Common.Tools.NuGet;
using Cake.Common.Tools.NuGet.Install;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LocalStack.Build
{
    public sealed class BuildContext : FrostingContext
    {
        public BuildContext(ICakeContext context) : base(context)
        {
            BuildConfiguration = context.Argument("config", "Release");
            ForceBuild = context.Argument("force-build", false);
            ForceRestore = context.Argument("force-restore", false);
            BuildNumber = context.Argument<int>("buildnumber", 1);
            SkipFunctionalTest = context.Argument("skipFunctionalTest", "1");

            SolutionRoot = context.Directory("../../");
            SrcPath = SolutionRoot + context.Directory("src");
            TestsPath = SolutionRoot + context.Directory("tests");
            BuildPath = SolutionRoot + context.Directory("build");
            ArtifactOutput = SolutionRoot + context.Directory("artifacts");
            ArtifactExtensionsOutput = SolutionRoot + context.Directory("artifacts-extensions");
            LocalStackClientFolder = SrcPath + context.Directory("LocalStack.Client");
            LocalStackClientExtFolder = SrcPath + context.Directory("LocalStack.Client.Extensions");

            SlnFilePath = SolutionRoot + context.File("LocalStack.sln");
            LocalStackClientProjFile = LocalStackClientFolder + context.File("LocalStack.Client.csproj");
            LocalStackClientExtProjFile = LocalStackClientExtFolder + context.File("LocalStack.Client.Extensions.csproj");
        }

        public string BuildConfiguration { get; }

        public bool ForceBuild { get; }

        public bool ForceRestore { get; }

        public string TestingMode { get; }

        public string SkipFunctionalTest { get; set; }

        public int BuildNumber { get; set; }

        public ConvertableFilePath SlnFilePath { get; set; }

        public ConvertableDirectoryPath SolutionRoot { get; }

        public ConvertableDirectoryPath SrcPath { get; }

        public ConvertableDirectoryPath TestsPath { get; }

        public ConvertableDirectoryPath BuildPath { get; }

        public ConvertableDirectoryPath ArtifactOutput { get; set; }

        public ConvertableDirectoryPath ArtifactExtensionsOutput { get; set; }

        public ConvertableDirectoryPath LocalStackClientFolder { get; set; }

        public ConvertableDirectoryPath LocalStackClientExtFolder { get; set; }

        public ConvertableFilePath LocalStackClientProjFile { get; set; }

        public ConvertableFilePath LocalStackClientExtProjFile { get; set; }

        public void InstallXUnitNugetPackage()
        {
            var nugetInstallSettings = new NuGetInstallSettings
            {
                Version = "2.4.1",
                Verbosity = NuGetVerbosity.Normal,
                OutputDirectory = "testrunner",
                WorkingDirectory = "."
            };

            this.NuGetInstall("xunit.runner.console", nugetInstallSettings);
        }

        public IList<TestProjMetadata> GetProjMetadata()
        {
            DirectoryPath testsRoot = this.MakeAbsolute(this.Directory(TestsPath));
            List<FilePath> csProjs = this.GetFiles($"{testsRoot}/**/*.csproj").Where(fp => fp.FullPath.EndsWith("Tests.csproj")).ToList();

            IList<TestProjMetadata> testProjMetadatas = new List<TestProjMetadata>();

            foreach (var csProj in csProjs)
            {
                string csProjPath = csProj.FullPath;

                string[] targetFrameworks = this.GetProjectTargetFrameworks(csProjPath);
                string directoryPath = csProj.GetDirectory().FullPath;
                string assemblyName = GetAssemblyName(csProjPath);

                var testProjMetadata = new TestProjMetadata(directoryPath, csProjPath, targetFrameworks, assemblyName);
                testProjMetadatas.Add(testProjMetadata);
            }

            return testProjMetadatas;
        }

        public string[] GetProjectTargetFrameworks(string csprojPath)
        {
            FilePath file = this.MakeAbsolute(this.File(csprojPath));
            string project = System.IO.File.ReadAllText(file.FullPath, Encoding.UTF8);

            bool multipleFrameworks = project.Contains("<TargetFrameworks>");
            string startElement = multipleFrameworks ? "<TargetFrameworks>" : "<TargetFramework>";
            string endElement = multipleFrameworks ? "</TargetFrameworks>" : "</TargetFramework>";

            int startIndex = project.IndexOf(startElement, StringComparison.Ordinal) + startElement.Length;
            int endIndex = project.IndexOf(endElement, startIndex, StringComparison.Ordinal);

            string targetFrameworks = project.Substring(startIndex, endIndex - startIndex);

            return targetFrameworks.Split(';');
        }

        public string GetAssemblyName(string csprojPath)
        {
            FilePath file = this.MakeAbsolute(this.File(csprojPath));
            string project = System.IO.File.ReadAllText(file.FullPath, Encoding.UTF8);

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
                int startIndex = csprojPath.LastIndexOf("/", StringComparison.Ordinal) + 1;
                int endIndex = csprojPath.IndexOf(".csproj", startIndex, StringComparison.Ordinal);

                assemblyName = csprojPath.Substring(startIndex, endIndex - startIndex);
            }

            return assemblyName;
        }

        public void RunXunitUsingMono(string targetFramework, string assemblyPath)
        {
            int exitCode = this.StartProcess("mono", new ProcessSettings
            {
                Arguments = $"./testrunner/xunit.runner.console.2.4.1/tools/{targetFramework}/xunit.console.exe {assemblyPath}"
            });

            if (exitCode != 0)
            {
                throw new InvalidOperationException($"Exit code: {exitCode}");
            }
        }
    }
}
