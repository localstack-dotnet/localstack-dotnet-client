#pragma warning disable MA0048 // File name must match type name - disabled because of readability

namespace LocalStack.Client.Functional.Tests.Scenarios.RealLife;

[Collection(nameof(LocalStackCollectionV37))]
public sealed class SnsToSqsScenarioV37 : BaseRealLife
{
    public SnsToSqsScenarioV37(TestFixture testFixture, LocalStackFixtureV37 localStackFixtureV37) : base(testFixture, localStackFixtureV37)
    {
    }
}

[Collection(nameof(LocalStackCollectionV43))]
public sealed class SnsToSqsScenarioV43 : BaseRealLife
{
    public SnsToSqsScenarioV43(TestFixture testFixture, LocalStackFixtureV43 localStackFixtureV43) : base(testFixture, localStackFixtureV43)
    {
    }
}