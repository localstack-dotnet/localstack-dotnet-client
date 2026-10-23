#pragma warning disable MA0048 // File name must match type name - disabled because of readability

namespace LocalStack.Client.Functional.Tests.Scenarios.DynamoDb;

[Collection(nameof(LocalStackCollectionV414))]
public sealed class DynamoDbScenarioV414 : BaseDynamoDbScenario
{
    public DynamoDbScenarioV414(TestFixture testFixture, LocalStackFixtureV414 localStackFixtureV414) : base(testFixture, localStackFixtureV414)
    {
    }
}

[Collection(nameof(LocalStackCollectionCurrent))]
[Trait("LocalStackAuth", "Required")]
public sealed class DynamoDbScenarioCurrent : BaseDynamoDbScenario
{
    public DynamoDbScenarioCurrent(TestFixture testFixture, LocalStackFixtureCurrent localStackFixtureCurrent) : base(testFixture, localStackFixtureCurrent)
    {
    }
}