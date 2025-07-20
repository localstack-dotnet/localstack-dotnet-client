[TaskName("init")]
public sealed class InitTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        ConsoleHelper.WriteRule("Initialization");
        ConsoleHelper.WriteHeader();

        context.StartProcess("dotnet", new ProcessSettings { Arguments = "--info" });

        if (!context.IsRunningOnUnix())
        {
            return;
        }

        context.StartProcess("git", new ProcessSettings { Arguments = "config --global core.autocrlf true" });
    }
}