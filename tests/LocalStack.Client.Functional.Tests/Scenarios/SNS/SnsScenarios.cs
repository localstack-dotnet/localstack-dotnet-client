#pragma warning disable MA0048 // File name must match type name - disabled because of readability

namespace LocalStack.Client.Functional.Tests.Scenarios.SNS;

[Collection(nameof(LocalStackCollectionV37))]
public sealed class SnsScenarioV37 : BaseSnsScenario
{
    public SnsScenarioV37(TestFixture testFixture, LocalStackFixtureV37 localStackFixtureV37) : base(testFixture, localStackFixtureV37)
    {
    }
}

[Collection(nameof(LocalStackCollectionV46))]
public sealed class SnsScenarioV46 : BaseSnsScenario
{
    public SnsScenarioV46(TestFixture testFixture, LocalStackFixtureV46 localStackFixtureV46) : base(testFixture, localStackFixtureV46)
    {
    }
}