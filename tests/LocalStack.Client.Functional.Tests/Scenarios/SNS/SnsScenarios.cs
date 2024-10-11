#pragma warning disable MA0048 // File name must match type name - disabled because of readability

namespace LocalStack.Client.Functional.Tests.Scenarios.SNS;

[Collection(nameof(LocalStackCollectionV131))]
public sealed class SnsScenarioV131 : BaseSnsScenario
{
    public SnsScenarioV131(TestFixture testFixture, LocalStackFixtureV131 localStackFixtureV131) : base(testFixture, localStackFixtureV131)
    {
    }
}

[Collection(nameof(LocalStackCollectionV23))]
public sealed class SnsScenarioV23 : BaseSnsScenario
{
    public SnsScenarioV23(TestFixture testFixture, LocalStackFixtureV23 localStackFixtureV23) : base(testFixture, localStackFixtureV23)
    {
    }
}

[Collection(nameof(LocalStackCollectionV34))]
public sealed class SnsScenarioV34 : BaseSnsScenario
{
    public SnsScenarioV34(TestFixture testFixture, LocalStackFixtureV34 localStackFixtureV34) : base(testFixture, localStackFixtureV34)
    {
    }
}

[Collection(nameof(LocalStackCollectionV38))]
public sealed class SnsScenarioV38 : BaseSnsScenario
{
    public SnsScenarioV38(TestFixture testFixture, LocalStackFixtureV38 localStackFixtureV38) : base(testFixture, localStackFixtureV38)
    {
    }
}