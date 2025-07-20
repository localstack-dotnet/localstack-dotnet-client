using LocalStack.Build.CakeTasks.Nuget.Services;

[TaskName("nuget-pack")]
public sealed class NugetPackTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        // Display header
        ConsoleHelper.WriteRule("Package Creation");

        // If no specific package ID is provided, pack all packages
        if (string.IsNullOrEmpty(context.PackageId))
        {
            PackAllPackages(context);
        }
        else
        {
            PackSinglePackage(context, context.PackageId);
        }

        ConsoleHelper.WriteRule();
    }

    private static void PackAllPackages(BuildContext context)
    {
        foreach (string packageId in context.PackageIdProjMap.Keys)
        {
            ConsoleHelper.WriteInfo($"Creating package: {packageId}");
            PackSinglePackage(context, packageId);
        }
    }

    private static void PackSinglePackage(BuildContext context, string packageId)
    {
        PackageOperations.PackSinglePackage(context, packageId);
    }
}