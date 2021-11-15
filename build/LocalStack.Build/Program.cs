﻿return new CakeHost()
       .UseContext<BuildContext>()
       .Run(args);

[TaskName("Default"),
 IsDependentOn(typeof(InitTask)),
 IsDependentOn(typeof(TestTask))]
public class DefaultTask : FrostingTask
{
}

[TaskName("init")]
public sealed class InitTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.StartProcess("dotnet", new ProcessSettings
        {
            Arguments = "--info"
        });

        if (!context.IsRunningOnUnix())
        {
            return;
        }

        context.StartProcess("git", new ProcessSettings
        {
            Arguments = "config --global core.autocrlf true"
        });

        context.StartProcess("mono", new ProcessSettings
        {
            Arguments = "--version"
        });

        context.InstallXUnitNugetPackage();
    }
}

[TaskName("build")]
public sealed class BuildTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetCoreBuild(context.SlnFilePath,
                                new DotNetCoreBuildSettings
                                {
                                    Configuration = context.BuildConfiguration
                                });
    }
}

[TaskName("test"),
 IsDependentOn(typeof(BuildTask))]
public sealed class TestTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        const string testResults = "results.trx";

        var settings = new DotNetCoreTestSettings
        {
            NoRestore = !context.ForceRestore,
            NoBuild = !context.ForceBuild,
            Configuration = context.BuildConfiguration
        };

        IList<ProjMetadata> projMetadata = context.GetProjMetadata();

        foreach (ProjMetadata testProj in projMetadata)
        {
            string testProjectPath = testProj.CsProjPath;

            context.Warning($"Target Frameworks {string.Join(" ", testProj.TargetFrameworks)}");

            foreach (string targetFramework in testProj.TargetFrameworks)
            {
                if (context.SkipFunctionalTest == "1" && testProj.AssemblyName == "LocalStack.Client.Functional.Tests")
                {
                    context.Warning("Skipping Functional Tests");
                    continue;
                }

                context.Warning($"Running {targetFramework.ToUpper()} tests for {testProj.AssemblyName}");
                settings.Framework = targetFramework;

                if (context.IsRunningOnUnix() && targetFramework == "net461")
                {
                    context.RunXunitUsingMono(targetFramework, $"{testProj.DirectoryPath}/bin/{context.BuildConfiguration}/{targetFramework}/{testProj.AssemblyName}.dll");
                }
                else
                {
                    string testFilePrefix = targetFramework.Replace(".", "-");
                    settings.ArgumentCustomization = args => args.Append($" --logger \"trx;LogFileName={testFilePrefix}_{testResults}\"");
                    context.DotNetCoreTest(testProjectPath, settings);
                }
            }
        }
    }
}

[TaskName("nuget-pack")]
public sealed class NugetPackTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        if (!System.IO.Directory.Exists(context.ArtifactOutput))
        {
            System.IO.Directory.CreateDirectory(context.ArtifactOutput);
        }

        var settings = new DotNetCorePackSettings
        {
            Configuration = context.BuildConfiguration, 
            OutputDirectory = context.ArtifactOutput, 
            MSBuildSettings = new DotNetCoreMSBuildSettings()
        };

        settings.MSBuildSettings.SetVersion(context.GetProjectVersion());

        context.DotNetCorePack(context.LocalStackClientProjFile, settings);
    }
}
