[TaskName("nuget-prepare-extensions")]
public sealed class NugetPrepareExtensionsTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        ConsoleHelper.WriteRule("Prepare Extensions Project");

        // Validate that this is for Extensions package
        if (context.PackageId != BuildContext.LocalStackClientExtensionsProjName)
        {
            throw new InvalidOperationException($"This task is only for {BuildContext.LocalStackClientExtensionsProjName}, but received: {context.PackageId}");
        }

        // Client version must be explicitly provided
        if (string.IsNullOrWhiteSpace(context.ClientVersion))
        {
            throw new InvalidOperationException("Client version must be specified via --client-version parameter. This task does not generate versions automatically.");
        }

        ConsoleHelper.WriteInfo($"Preparing Extensions project for LocalStack.Client v{context.ClientVersion}");
        ConsoleHelper.WriteInfo($"Package source: {context.PackageSource}");

        PrepareExtensionsProject(context, context.ClientVersion);

        ConsoleHelper.WriteSuccess("Extensions project preparation completed!");
        ConsoleHelper.WriteRule();
    }

    private static void PrepareExtensionsProject(BuildContext context, string version)
    {
        ConsoleHelper.WriteProcessing("Updating Extensions project dependencies...");

        try
        {
            // Use the Extensions project file path directly
            string extensionsProject = context.LocalStackClientExtProjFile.Path.FullPath;
            var clientProjectRef = context.File(context.LocalStackClientProjFile.Path.FullPath);

            // Remove project reference
            context.DotNetRemoveReference(extensionsProject, [clientProjectRef]);
            ConsoleHelper.WriteInfo("Removed project reference to LocalStack.Client");

            // Add package reference with specific version and source
            var packageSettings = new DotNetPackageAddSettings
            {
                Version = version
            };

            // Add source if not NuGet (GitHub Packages, MyGet, etc.)
            if (context.PackageSource != BuildContext.NuGetPackageSource)
            {
                packageSettings.Source = context.PackageSourceMap[context.PackageSource];
                ConsoleHelper.WriteInfo($"Using package source: {context.PackageSource}");
            }

            context.DotNetAddPackage(BuildContext.LocalStackClientProjName, extensionsProject, packageSettings);
            ConsoleHelper.WriteSuccess($"Added package reference for {BuildContext.LocalStackClientProjName} v{version}");
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteError($"Failed to prepare Extensions project: {ex.Message}");
            throw;
        }
    }
}