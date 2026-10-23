#pragma warning disable MA0048 // File name must match type name - disabled because of readability

namespace LocalStack.Client.Functional.Tests.Scenarios.RealLife;

[Collection(nameof(LocalStackCollectionV414))]
public sealed class SnsToSqsScenarioV414 : BaseRealLife
{
    public SnsToSqsScenarioV414(TestFixture testFixture, LocalStackFixtureV414 localStackFixtureV414) : base(testFixture, localStackFixtureV414)
    {
    }
}

[Collection(nameof(LocalStackCollectionCurrent))]
[Trait("LocalStackAuth", "Required")]
public sealed class SnsToSqsScenarioCurrent : BaseRealLife
{
    public SnsToSqsScenarioCurrent(TestFixture testFixture, LocalStackFixtureCurrent localStackFixtureCurrent) : base(testFixture, localStackFixtureCurrent)
    {
    }
}