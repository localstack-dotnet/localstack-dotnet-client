#pragma warning disable MA0048 // File name must match type name - disabled because of readability

namespace LocalStack.Client.Functional.Tests.Scenarios.DynamoDb;

[Collection(nameof(LocalStackCollectionV131))]
public class DynamoDbScenarioV131 : BaseDynamoDbScenario
{
    public DynamoDbScenarioV131(TestFixture testFixture, LocalStackFixtureV131 localStackFixtureV131) : base(testFixture, localStackFixtureV131)
    {
    }
}

[Collection(nameof(LocalStackCollectionV23))]
public sealed class DynamoDbScenarioV23 : BaseDynamoDbScenario
{
    public DynamoDbScenarioV23(TestFixture testFixture, LocalStackFixtureV23 localStackFixtureV23) : base(testFixture, localStackFixtureV23)
    {
    }
}

[Collection(nameof(LocalStackCollectionV34))]
public sealed class DynamoDbScenarioV34 : BaseDynamoDbScenario
{
    public DynamoDbScenarioV34(TestFixture testFixture, LocalStackFixtureV34 localStackFixtureV34) : base(testFixture, localStackFixtureV34)
    {
    }
}

[Collection(nameof(LocalStackCollectionV38))]
public sealed class DynamoDbScenarioV38 : BaseDynamoDbScenario
{
    public DynamoDbScenarioV38(TestFixture testFixture, LocalStackFixtureV38 localStackFixtureV38) : base(testFixture, localStackFixtureV38)
    {
    }
}