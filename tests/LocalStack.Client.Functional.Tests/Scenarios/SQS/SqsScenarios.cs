#pragma warning disable MA0048 // File name must match type name - disabled because of readability

namespace LocalStack.Client.Functional.Tests.Scenarios.SQS;

[Collection(nameof(LocalStackCollectionV414))]
public sealed class SqsScenarioV414 : BaseSqsScenario
{
    public SqsScenarioV414(TestFixture testFixture, LocalStackFixtureV414 localStackFixtureV414) : base(testFixture, localStackFixtureV414)
    {
    }
}

[Collection(nameof(LocalStackCollectionCurrent))]
[Trait("LocalStackAuth", "Required")]
public sealed class SqsScenarioCurrent : BaseSqsScenario
{
    public SqsScenarioCurrent(TestFixture testFixture, LocalStackFixtureCurrent localStackFixtureCurrent) : base(testFixture, localStackFixtureCurrent)
    {
    }
}