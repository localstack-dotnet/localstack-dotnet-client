namespace LocalStack.Client.Functional.Tests.Scenarios.SQS;

[Collection(nameof(LocalStackCollectionV131))]
public sealed class SqsScenarioV131 : BaseSqsScenario
{
    public SqsScenarioV131(TestFixture testFixture, LocalStackFixtureV131 localStackFixtureV131) : base(testFixture, localStackFixtureV131)
    {
    }
}

[Collection(nameof(LocalStackCollectionV20))]
public sealed class SqsScenarioV20 : BaseSqsScenario
{
    public SqsScenarioV20(TestFixture testFixture, LocalStackFixtureV20 localStackFixtureV20) : base(testFixture, localStackFixtureV20)
    {
    }
}

[Collection(nameof(LocalStackCollectionV22))]
public sealed class SqsScenarioV22 : BaseSqsScenario
{
    public SqsScenarioV22(TestFixture testFixture, LocalStackFixtureV22 localStackFixtureV22) : base(testFixture, localStackFixtureV22)
    {
    }
}

[Collection(nameof(LocalStackLegacyCollection))]
public sealed class SqsLegacyScenario : BaseSqsScenario
{
    public SqsLegacyScenario(TestFixture testFixture, LocalStackLegacyFixture localStackLegacyFixture) : base(
        testFixture, localStackLegacyFixture, TestConstants.LegacyLocalStackConfig, true)
    {
    }
}