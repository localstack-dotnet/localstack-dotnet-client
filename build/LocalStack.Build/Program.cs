#pragma warning disable CA1515 // Consider making public types internal

return new CakeHost().UseContext<BuildContext>().Run(args);

[TaskName("Default"), IsDependentOn(typeof(TestTask))]
public class DefaultTask : FrostingTask;