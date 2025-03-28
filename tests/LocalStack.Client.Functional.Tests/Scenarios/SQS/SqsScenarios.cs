#pragma warning disable MA0048 // File name must match type name - disabled because of readability

namespace LocalStack.Client.Functional.Tests.Scenarios.SQS;

[Collection(nameof(LocalStackCollectionV37))]
public sealed class SqsScenarioV37 : BaseSqsScenario
{
    public SqsScenarioV37(TestFixture testFixture, LocalStackFixtureV37 localStackFixtureV37) : base(testFixture, localStackFixtureV37)
    {
    }
}

[Collection(nameof(LocalStackCollectionV43))]
public sealed class SqsScenarioV43 : BaseSqsScenario
{
    public SqsScenarioV43(TestFixture testFixture, LocalStackFixtureV43 localStackFixtureV43) : base(testFixture, localStackFixtureV43)
    {
    }
}