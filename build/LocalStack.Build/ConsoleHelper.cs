#pragma warning disable CA1515 // Consider making public types internal
#pragma warning disable CA1055 // Change the return type of method 'ConsoleHelper.GetDownloadUrl(string, string, string, [string])' from 'string' to 'System.Uri'

namespace LocalStack.Build;

/// <summary>
/// Helper class for rich console output using Spectre.Console
/// </summary>
public static class ConsoleHelper
{
    /// <summary>
    /// Displays a large LocalStack.NET header with FigletText
    /// </summary>
    public static void WriteHeader()
    {
        AnsiConsole.Write(new FigletText("LocalStack.NET").LeftJustified().Color(Color.Blue));
    }

    /// <summary>
    /// Displays a success message with green checkmark
    /// </summary>
    /// <param name="message">The success message to display</param>
    public static void WriteSuccess(string message)
    {
        AnsiConsole.MarkupLine($"[green]✅ {message.EscapeMarkup()}[/]");
    }

    /// <summary>
    /// Displays a warning message with yellow warning symbol
    /// </summary>
    /// <param name="message">The warning message to display</param>
    public static void WriteWarning(string message)
    {
        AnsiConsole.MarkupLine($"[yellow]⚠️  {message.EscapeMarkup()}[/]");
    }

    /// <summary>
    /// Displays an error message with red X symbol
    /// </summary>
    /// <param name="message">The error message to display</param>
    public static void WriteError(string message)
    {
        AnsiConsole.MarkupLine($"[red]❌ {message.EscapeMarkup()}[/]");
    }

    /// <summary>
    /// Displays an informational message with blue info symbol
    /// </summary>
    /// <param name="message">The info message to display</param>
    public static void WriteInfo(string message)
    {
        AnsiConsole.MarkupLine($"[cyan]ℹ️  {message.EscapeMarkup()}[/]");
    }

    /// <summary>
    /// Displays a processing message with gear symbol
    /// </summary>
    /// <param name="message">The processing message to display</param>
    public static void WriteProcessing(string message)
    {
        AnsiConsole.MarkupLine($"[yellow]🔧 {message.EscapeMarkup()}[/]");
    }

    /// <summary>
    /// Displays a package-related message with package symbol
    /// </summary>
    /// <param name="message">The package message to display</param>
    public static void WritePackage(string message)
    {
        AnsiConsole.MarkupLine($"[cyan]📦 {message.EscapeMarkup()}[/]");
    }

    /// <summary>
    /// Displays an upload/publish message with rocket symbol
    /// </summary>
    /// <param name="message">The upload message to display</param>
    public static void WriteUpload(string message)
    {
        AnsiConsole.MarkupLine($"[green]📤 {message.EscapeMarkup()}[/]");
    }

    /// <summary>
    /// Creates and displays a package information table
    /// </summary>
    /// <param name="packageId">The package identifier</param>
    /// <param name="version">The package version</param>
    /// <param name="targetFrameworks">The target frameworks</param>
    /// <param name="buildConfig">The build configuration</param>
    /// <param name="packageSource">The package source</param>
    public static void WritePackageInfoTable(string packageId, string version, string targetFrameworks, string buildConfig, string packageSource)
    {
        var table = new Table().Border(TableBorder.Rounded)
                               .BorderColor(Color.Grey)
                               .AddColumn(new TableColumn("[yellow]Property[/]").Centered())
                               .AddColumn(new TableColumn("[cyan]Value[/]").LeftAligned())
                               .AddRow("Package ID", packageId.EscapeMarkup())
                               .AddRow("Version", version.EscapeMarkup())
                               .AddRow("Target Frameworks", targetFrameworks.EscapeMarkup())
                               .AddRow("Build Configuration", buildConfig.EscapeMarkup())
                               .AddRow("Package Source", packageSource.EscapeMarkup());

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    /// <summary>
    /// Creates and displays a publication summary panel
    /// </summary>
    /// <param name="packageId">The package identifier</param>
    /// <param name="version">The package version</param>
    /// <param name="packageSource">The package source</param>
    /// <param name="downloadUrl">The download URL</param>
#pragma warning disable MA0006 // Use String.Create instead of string concatenation
    public static void WritePublicationSummary(string packageId, string version, string packageSource, string downloadUrl)
    {
        var panel = new Panel(new Markup($"""
                                          [bold]📦 Package:[/] {packageId.EscapeMarkup()}
                                          [bold]🏷️ Version:[/] {version.EscapeMarkup()}
                                          [bold]🎯 Published to:[/] {packageSource.EscapeMarkup()}
                                          [bold]🔗 Download URL:[/] [link]{downloadUrl.EscapeMarkup()}[/]
                                          """)).Header(new PanelHeader("[bold green]✅ Publication Complete[/]").Centered())
                                               .BorderColor(Color.Green)
                                               .Padding(1, 1);

        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();
    }

    /// <summary>
    /// Executes a function with a progress bar
    /// </summary>
    /// <param name="description">Description of the operation</param>
    /// <param name="action">The action to execute with progress context</param>
    public static void WithProgress(string description, Action<ProgressContext> action)
    {
        AnsiConsole.Progress()
                   .Start(ctx =>
                   {
                       var task = ctx.AddTask($"[green]{description.EscapeMarkup()}[/]");
                       action(ctx);
                       task.Increment(100);
                   });
    }

    /// <summary>
    /// Displays a rule separator with optional text
    /// </summary>
    /// <param name="title">Optional title for the rule</param>
    public static void WriteRule(string title = "")
    {
        var rule = string.IsNullOrEmpty(title) ? new Rule() : new Rule($"[bold blue]{title.EscapeMarkup()}[/]");

        AnsiConsole.Write(rule);
    }

    /// <summary>
    /// Displays version generation information
    /// </summary>
    /// <param name="baseVersion">The base version from Directory.Build.props</param>
    /// <param name="finalVersion">The final generated version with metadata</param>
    /// <param name="buildDate">The build date</param>
    /// <param name="commitSha">The git commit SHA</param>
    /// <param name="branchName">The git branch name</param>
    public static void WriteVersionInfo(string baseVersion, string finalVersion, string buildDate, string commitSha, string branchName)
    {
        var table = new Table().Border(TableBorder.Simple)
                               .BorderColor(Color.Grey)
                               .AddColumn(new TableColumn("[yellow]Version Component[/]").Centered())
                               .AddColumn(new TableColumn("[cyan]Value[/]").LeftAligned())
                               .AddRow("Base Version", baseVersion.EscapeMarkup())
                               .AddRow("Build Date", buildDate.EscapeMarkup())
                               .AddRow("Commit SHA", commitSha.EscapeMarkup())
                               .AddRow("Branch", branchName.EscapeMarkup())
                               .AddRow("[bold]Final Version[/]", $"[bold green]{finalVersion.EscapeMarkup()}[/]");

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    /// <summary>
    /// Generates a download URL based on package source
    /// </summary>
    /// <param name="packageSource">The package source (github, nuget, myget)</param>
    /// <param name="packageId">The package identifier</param>
    /// <param name="version">The package version</param>
    /// <param name="repositoryOwner">The repository owner (for GitHub packages)</param>
    /// <returns>The download URL</returns>
    public static string GetDownloadUrl(string packageSource, string packageId, string version, string repositoryOwner = "localstack-dotnet")
    {
        return packageSource?.ToUpperInvariant() switch
        {
            "GITHUB" => $"https://github.com/{repositoryOwner}/localstack-dotnet-client/packages",
            "NUGET" => $"https://www.nuget.org/packages/{packageId}/{version}",
            "MYGET" => $"https://www.myget.org/packages/{packageId}",
            _ => "Package published successfully",
        };
    }
}