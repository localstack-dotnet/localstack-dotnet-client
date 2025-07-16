using LocalStack.Build.CakeTasks.Nuget.Services;

[TaskName("nuget-push")]
public sealed class NugetPushTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        ConsoleHelper.WriteRule("Package Publishing");

        PackageOperations.PublishSinglePackage(context, context.PackageId);

        ConsoleHelper.WriteRule();
    }
}