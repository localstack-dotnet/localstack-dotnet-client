[TaskName("build"), IsDependentOn(typeof(InitTask))]
public sealed class BuildTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        ConsoleHelper.WriteInfo($"AWS SDK setup track: {context.AwsSetupTrack}");

        context.DotNetBuild(context.SlnFilePath, new DotNetBuildSettings
        {
            Configuration = context.BuildConfiguration,
            MSBuildSettings = new DotNetMSBuildSettings().WithProperty("AwsSetupTrack", context.AwsSetupTrack),
        });
    }
}