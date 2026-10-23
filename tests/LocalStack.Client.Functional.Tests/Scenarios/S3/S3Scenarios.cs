#pragma warning disable MA0048 // File name must match type name - disabled because of readability

namespace LocalStack.Client.Functional.Tests.Scenarios.S3;

[Collection(nameof(LocalStackCollectionV414))]
public sealed class S3ScenarioV414 : BaseS3Scenario
{
    public S3ScenarioV414(TestFixture testFixture, LocalStackFixtureV414 localStackFixtureV414) : base(testFixture, localStackFixtureV414)
    {
    }
}

[Collection(nameof(LocalStackCollectionCurrent))]
[Trait("LocalStackAuth", "Required")]
public sealed class S3ScenarioCurrent : BaseS3Scenario
{
    public S3ScenarioCurrent(TestFixture testFixture, LocalStackFixtureCurrent localStackFixtureCurrent) : base(testFixture, localStackFixtureCurrent)
    {
    }
}