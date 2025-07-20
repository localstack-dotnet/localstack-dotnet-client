[TaskName("build"), IsDependentOn(typeof(InitTask))]
public sealed class BuildTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetBuild(context.SlnFilePath, new DotNetBuildSettings { Configuration = context.BuildConfiguration });
    }
}