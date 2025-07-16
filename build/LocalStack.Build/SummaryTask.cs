using System.Globalization;

[TaskName("workflow-summary")]
public sealed class SummaryTask : FrostingTask<BuildContext>
{
    private const string GitHubOwner = "localstack-dotnet";

    public override void Run(BuildContext context)
    {
        ConsoleHelper.WriteRule("Build Summary");

        GenerateBuildSummary(context);
        GenerateInstallationInstructions(context);
        GenerateMetadataTable(context);

        ConsoleHelper.WriteRule();
    }

    private static void GenerateBuildSummary(BuildContext context)
    {
        var panel = new Panel(GetSummaryContent(context))
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Green)
            .Header("[bold green]✅ Build Complete[/]")
            .HeaderAlignment(Justify.Center);

        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();
    }

    private static string GetSummaryContent(BuildContext context)
    {
        var content = new StringBuilder();

        if (string.IsNullOrEmpty(context.PackageId))
        {
            // Summary for all packages
            content.AppendLine(CultureInfo.InvariantCulture, $"[bold]📦 Packages Built:[/]");
            
            foreach (string packageId in context.PackageIdProjMap.Keys)
            {
                string version = GetPackageVersion(context, packageId);
                content.AppendLine(CultureInfo.InvariantCulture, $"  • [cyan]{packageId}[/] [yellow]v{version}[/]");
            }
        }
        else
        {
            // Summary for specific package
            string version = GetPackageVersion(context, context.PackageId);
            content.AppendLine(CultureInfo.InvariantCulture, $"[bold]📦 Package:[/] [cyan]{context.PackageId}[/]");
            content.AppendLine(CultureInfo.InvariantCulture, $"[bold]🏷️  Version:[/] [yellow]{version}[/]");
            content.AppendLine(CultureInfo.InvariantCulture, $"[bold]🎯 Target:[/] [blue]{context.PackageSource}[/]");
            content.AppendLine(CultureInfo.InvariantCulture, $"[bold]⚙️  Config:[/] [green]{context.BuildConfiguration}[/]");
        }

        return content.ToString().TrimEnd();
    }

    private static void GenerateInstallationInstructions(BuildContext context)
    {
        var panel = new Panel(GetInstallationContent(context))
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Blue)
            .Header("[bold blue]🚀 Installation Instructions[/]")
            .HeaderAlignment(Justify.Center);

        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();
    }

    private static string GetInstallationContent(BuildContext context)
    {
        var content = new StringBuilder();

        if (context.PackageSource == BuildContext.GitHubPackageSource)
        {
            content.AppendLine("[bold]1. Add GitHub Packages source:[/]");
            content.AppendLine(CultureInfo.InvariantCulture, $"[grey]dotnet nuget add source https://nuget.pkg.github.com/{GitHubOwner}/index.json \\[/]");
            content.AppendLine("[grey]  --name github-localstack \\[/]");
            content.AppendLine("[grey]  --username YOUR_USERNAME \\[/]");
            content.AppendLine("[grey]  --password YOUR_GITHUB_TOKEN[/]");
            content.AppendLine();
        }

        content.AppendLine("[bold]2. Install package(s):[/]");

        if (string.IsNullOrEmpty(context.PackageId))
        {
            // Installation for all packages
            foreach (string packageId in context.PackageIdProjMap.Keys)
            {
                string version = GetPackageVersion(context, packageId);
                content.AppendLine(GetInstallCommand(packageId, version, context.PackageSource));
            }
        }
        else
        {
            // Installation for specific package
            string version = GetPackageVersion(context, context.PackageId);
            content.AppendLine(GetInstallCommand(context.PackageId, version, context.PackageSource));
        }

        return content.ToString().TrimEnd();
    }

    private static string GetInstallCommand(string packageId, string version, string packageSource)
    {
        string sourceFlag = packageSource == BuildContext.GitHubPackageSource ? " --source github-localstack" : "";
        return $"[grey]dotnet add package {packageId} --version {version}{sourceFlag}[/]";
    }

    private static void GenerateMetadataTable(BuildContext context)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Grey)
            .Title("[bold]📊 Build Metadata[/]")
            .AddColumn("[yellow]Property[/]")
            .AddColumn("[cyan]Value[/]");

        // Add build information
        table.AddRow("Build Date", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC", CultureInfo.InvariantCulture));
        table.AddRow("Build Configuration", context.BuildConfiguration);
        
        if (context.UseDirectoryPropsVersion)
        {
            table.AddRow("Version Source", "Directory.Build.props (Dynamic)");
            table.AddRow("Branch Name", context.BranchName);
            
            try
            {
                // Simply skip git commit info since the method is private
                table.AddRow("Git Commit", "See build output");
            }
            catch
            {
                table.AddRow("Git Commit", "Not available");
            }
        }
        else
        {
            table.AddRow("Version Source", "Manual");
        }

        // Add package information
        if (!string.IsNullOrEmpty(context.PackageId))
        {
            string targetFrameworks = context.GetPackageTargetFrameworks(context.PackageId);
            table.AddRow("Target Frameworks", targetFrameworks);
            
            string downloadUrl = ConsoleHelper.GetDownloadUrl(context.PackageSource, context.PackageId, GetPackageVersion(context, context.PackageId));
            table.AddRow("Download URL", downloadUrl);
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    private static string GetPackageVersion(BuildContext context, string packageId)
    {
        return packageId switch
        {
            BuildContext.LocalStackClientProjName => context.GetProjectVersion(),
            BuildContext.LocalStackClientExtensionsProjName => context.GetExtensionProjectVersion(),
            _ => "Unknown",
        };
    }
}
