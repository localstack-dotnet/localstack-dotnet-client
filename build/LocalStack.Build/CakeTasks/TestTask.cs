﻿[TaskName("tests"), IsDependentOn(typeof(BuildTask))]
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

            ConsoleHelper.WriteInfo($"Target Frameworks: {targetFrameworks}");

            foreach (string targetFramework in testProj.TargetFrameworks)
            {
                if (context.SkipFunctionalTest && testProj.AssemblyName == "LocalStack.Client.Functional.Tests")
                {
                    ConsoleHelper.WriteWarning("Skipping Functional Tests");

                    continue;
                }

                ConsoleHelper.WriteRule($"Running {targetFramework.ToUpper(System.Globalization.CultureInfo.CurrentCulture)} tests for {testProj.AssemblyName}");
                settings.Framework = targetFramework;

                if (testProj.AssemblyName == "LocalStack.Client.Functional.Tests")
                {
                    ConsoleHelper.WriteProcessing("Deleting running docker containers");

                    try
                    {
                        string psOutput = context.DockerPs(new DockerContainerPsSettings() { All = true, Quiet = true });

                        if (!string.IsNullOrEmpty(psOutput))
                        {
                            ConsoleHelper.WriteInfo($"Found containers: {psOutput}");

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
                    ConsoleHelper.WriteInfo($"Running .NET Framework tests on {platform}");
                }

                string testFilePrefix = targetFramework.Replace('.', '-');
                settings.ArgumentCustomization = args => args.Append($" --logger \"trx;LogFileName={testFilePrefix}_{testResults}\"");
                context.DotNetTest(testProjectPath, settings);

                ConsoleHelper.WriteSuccess($"Completed {targetFramework} tests for {testProj.AssemblyName}");
            }
        }
    }
}