#pragma warning disable MA0048 // File name must match type name - disabled because of readability

namespace LocalStack.Client.Functional.Tests.Scenarios.DynamoDb;

[Collection(nameof(LocalStackCollectionV37))]
public sealed class DynamoDbScenarioV37 : BaseDynamoDbScenario
{
    public DynamoDbScenarioV37(TestFixture testFixture, LocalStackFixtureV37 localStackFixtureV37) : base(testFixture, localStackFixtureV37)
    {
    }
}

[Collection(nameof(LocalStackCollectionV46))]
public sealed class DynamoDbScenarioV46 : BaseDynamoDbScenario
{
    public DynamoDbScenarioV46(TestFixture testFixture, LocalStackFixtureV46 localStackFixtureV46) : base(testFixture, localStackFixtureV46)
    {
    }
}