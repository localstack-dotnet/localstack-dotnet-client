#pragma warning disable MA0048 // File name must match type name - disabled because of readability

namespace LocalStack.Client.Functional.Tests.Scenarios.S3;

[Collection(nameof(LocalStackCollectionV37))]
public sealed class S3ScenarioV37 : BaseS3Scenario
{
    public S3ScenarioV37(TestFixture testFixture, LocalStackFixtureV37 localStackFixtureV37) : base(testFixture, localStackFixtureV37)
    {
    }
}

[Collection(nameof(LocalStackCollectionV43))]
public sealed class S3ScenarioV43 : BaseS3Scenario
{
    public S3ScenarioV43(TestFixture testFixture, LocalStackFixtureV43 localStackFixtureV43) : base(testFixture, localStackFixtureV43)
    {
    }
}