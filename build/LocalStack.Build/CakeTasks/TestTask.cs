[TaskName("tests"), IsDependentOn(typeof(BuildTask))]
public sealed class TestTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        const string testResults = "results.trx";

        var settings = new DotNetTestSettings
        {
            NoRestore = !context.ForceRestore,
            NoBuild = !context.ForceBuild,
            Configuration = context.BuildConfiguration,
            Blame = true,
            // Must match the track the solution was built with, otherwise dotnet test re-evaluates
            // the project with a different AWSSDK.Extensions.NETCore.Setup version than was compiled.
            MSBuildSettings = new DotNetMSBuildSettings().WithProperty("AwsSetupTrack", context.AwsSetupTrack),
        };

        IEnumerable<ProjMetadata> projMetadata = context.GetProjMetadata();
        string netFrameworkTargetFramework = context.LocalStackNetFrameworkTargetFramework;

        foreach (ProjMetadata testProj in projMetadata)
        {
            string testProjectPath = testProj.CsProjPath;
            string targetFrameworks = string.Join(',', testProj.TargetFrameworks);

            ConsoleHelper.WriteInfo($"Target Frameworks: {targetFrameworks}");

            if (testProj.RunsAsPublishedExecutable)
            {
                ConsoleHelper.WriteWarning($"Skipping {testProj.AssemblyName}: CI publishes and runs it as a native executable");

                continue;
            }

            foreach (string targetFramework in testProj.TargetFrameworks)
            {

                if (context.SkipFunctionalTest && testProj.AssemblyName == "LocalStack.Client.Functional.Tests")
                {
                    ConsoleHelper.WriteWarning("Skipping Functional Tests");

                    continue;
                }

                ConsoleHelper.WriteRule($"Running {targetFramework.ToUpper(System.Globalization.CultureInfo.CurrentCulture)} tests for {testProj.AssemblyName}");
                settings.Framework = targetFramework;

                if (testProj.AssemblyName == "LocalStack.Client.Functional.Tests" && context.SkipAuthenticatedFunctionalTest)
                {
                    settings.Filter = "LocalStackAuth!=Required";
                    ConsoleHelper.WriteWarning("Skipping functional tests that need LOCALSTACK_AUTH_TOKEN (trait LocalStackAuth=Required)");
                }
                else
                {
                    settings.Filter = null;
                }

                // The .NET 11 SDK's test platform runs .NET Framework tests on Windows only; Mono is no longer supported.
                if (targetFramework == netFrameworkTargetFramework && !context.IsRunningOnWindows())
                {
                    ConsoleHelper.WriteWarning($"Skipping {targetFramework} tests for {testProj.AssemblyName}: .NET Framework tests run on Windows only");

                    continue;
                }

                string testFilePrefix = targetFramework.Replace('.', '-');
                settings.ArgumentCustomization = args => args.Append($" --logger \"trx;LogFileName={testFilePrefix}_{testResults}\"");
                context.DotNetTest(testProjectPath, settings);

                ConsoleHelper.WriteSuccess($"Completed {targetFramework} tests for {testProj.AssemblyName}");
            }
        }
    }
}