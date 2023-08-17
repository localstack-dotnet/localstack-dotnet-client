namespace LocalStack.Client.Functional.Tests.Scenarios.S3;

[Collection(nameof(LocalStackCollectionV131))]
public sealed class S3ScenarioV131 : BaseS3Scenario
{
    public S3ScenarioV131(TestFixture testFixture, LocalStackFixtureV131 localStackFixtureV131) : base(testFixture, localStackFixtureV131)
    {
    }
}

[Collection(nameof(LocalStackCollectionV20))]
public sealed class S3ScenarioV20 : BaseS3Scenario
{
    public S3ScenarioV20(TestFixture testFixture, LocalStackFixtureV20 localStackFixtureV20) : base(testFixture, localStackFixtureV20)
    {
    }
}

[Collection(nameof(LocalStackCollectionV22))]
public sealed class S3ScenarioV22 : BaseS3Scenario
{
    public S3ScenarioV22(TestFixture testFixture, LocalStackFixtureV22 localStackFixtureV22) : base(testFixture, localStackFixtureV22)
    {
    }
}

[Collection(nameof(LocalStackLegacyCollection))]
public sealed class S3LegacyScenario : BaseS3Scenario
{
    public S3LegacyScenario(TestFixture testFixture, LocalStackLegacyFixture localStackLegacyFixture) : base(
        testFixture, localStackLegacyFixture, TestConstants.LegacyLocalStackConfig, true)
    {
    }
}