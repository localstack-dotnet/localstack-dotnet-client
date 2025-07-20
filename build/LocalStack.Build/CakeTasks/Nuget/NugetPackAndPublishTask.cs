using LocalStack.Build.CakeTasks.Nuget.Services;

[TaskName("nuget-pack-and-publish")]
public sealed class NugetPackAndPublishTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        ConsoleHelper.WriteRule("Pack & Publish Pipeline");

        string effectiveVersion = context.GetEffectivePackageVersion(context.PackageId);
        ConsoleHelper.WriteInfo($"Using consistent version: {effectiveVersion}");

        ConsoleHelper.WriteProcessing("Step 1: Creating package...");
        PackageOperations.PackSinglePackage(context, context.PackageId);

        ConsoleHelper.WriteProcessing("Step 2: Publishing package...");
        PackageOperations.PublishSinglePackage(context, context.PackageId);

        ConsoleHelper.WriteSuccess("Pack & Publish pipeline completed successfully!");
        ConsoleHelper.WriteRule();
    }
}