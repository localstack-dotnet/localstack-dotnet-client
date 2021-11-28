namespace LocalStack.Client.Functional.Tests.Scenarios.DynamoDb;

[Collection(nameof(LocalStackLegacyCollection))]
public class DynamoDbLegacyScenario : DynamoDbScenario
{
    public DynamoDbLegacyScenario(TestFixture testFixture)
        : base(testFixture, TestConstants.LegacyLocalStackConfig, true)
    {
    }
}
