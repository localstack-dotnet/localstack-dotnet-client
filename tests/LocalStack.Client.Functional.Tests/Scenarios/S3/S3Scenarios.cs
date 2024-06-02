#pragma warning disable MA0048 // File name must match type name - disabled because of readability

namespace LocalStack.Client.Functional.Tests.Scenarios.S3;

[Collection(nameof(LocalStackCollectionV131))]
public sealed class S3ScenarioV131 : BaseS3Scenario
{
    public S3ScenarioV131(TestFixture testFixture, LocalStackFixtureV131 localStackFixtureV131) : base(testFixture, localStackFixtureV131)
    {
    }
}

[Collection(nameof(LocalStackCollectionV23))]
public sealed class S3ScenarioV23 : BaseS3Scenario
{
    public S3ScenarioV23(TestFixture testFixture, LocalStackFixtureV23 localStackFixtureV23) : base(testFixture, localStackFixtureV23)
    {
    }
}

[Collection(nameof(LocalStackCollectionV34))]
public sealed class S3ScenarioV34 : BaseS3Scenario
{
    public S3ScenarioV34(TestFixture testFixture, LocalStackFixtureV34 localStackFixtureV34) : base(testFixture, localStackFixtureV34)
    {
    }
}