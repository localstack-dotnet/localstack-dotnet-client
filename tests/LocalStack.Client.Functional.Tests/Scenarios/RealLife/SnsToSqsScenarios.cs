#pragma warning disable MA0048 // File name must match type name - disabled because of readability

namespace LocalStack.Client.Functional.Tests.Scenarios.RealLife;

[Collection(nameof(LocalStackCollectionV37))]
public sealed class SnsToSqsScenarioV37 : BaseRealLife
{
    public SnsToSqsScenarioV37(TestFixture testFixture, LocalStackFixtureV37 localStackFixtureV37) : base(testFixture, localStackFixtureV37)
    {
    }
}

[Collection(nameof(LocalStackCollectionV46))]
public sealed class SnsToSqsScenarioV46 : BaseRealLife
{
    public SnsToSqsScenarioV46(TestFixture testFixture, LocalStackFixtureV46 localStackFixtureV46) : base(testFixture, localStackFixtureV46)
    {
    }
}