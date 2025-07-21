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

                if (targetFramework == "net462" && !context.IsRunningOnWindows())
                {
                    context.Warning("Skipping net462 tests on non-Windows platform");
                    continue;
                }

                string testFilePrefix = targetFramework.Replace('.', '-');
                settings.ArgumentCustomization = args => args.Append($" --logger \"trx;LogFileName={testFilePrefix}_{testResults}\"");
                context.DotNetTest(testProjectPath, settings);

                context.Warning("==============================================================");
            }
        }
    }
}