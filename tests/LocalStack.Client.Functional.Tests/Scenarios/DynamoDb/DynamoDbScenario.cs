#pragma warning disable MA0048 // File name must match type name - disabled because of readability

namespace LocalStack.Client.Functional.Tests.Scenarios.DynamoDb;

[Collection(nameof(LocalStackCollectionV131))]
public class DynamoDbScenarioV131 : BaseDynamoDbScenario
{
    public DynamoDbScenarioV131(TestFixture testFixture, LocalStackFixtureV131 localStackFixtureV131) : base(testFixture, localStackFixtureV131)
    {
    }
}

[Collection(nameof(LocalStackCollectionV20))]
public sealed class DynamoDbScenarioV20 : BaseDynamoDbScenario
{
    public DynamoDbScenarioV20(TestFixture testFixture, LocalStackFixtureV20 localStackFixtureV20) : base(testFixture, localStackFixtureV20)
    {
    }

    public override Task DynamoDbService_Should_Add_A_Record_To_A_DynamoDb_Table_Async()
    {
        return Task.CompletedTask;
    }

    public override Task DynamoDbService_Should_Create_A_DynamoDb_Table_Async()
    {
        return Task.CompletedTask;
    }

    public override Task DynamoDbService_Should_Delete_A_DynamoDb_Table_Async()
    {
        return Task.CompletedTask;
    }

    public override Task DynamoDbService_Should_List_Records_In_A_DynamoDb_Table_Async()
    {
        return Task.CompletedTask;
    }
}

[Collection(nameof(LocalStackCollectionV22))]
public sealed class DynamoDbScenarioV22 : BaseDynamoDbScenario
{
    public DynamoDbScenarioV22(TestFixture testFixture, LocalStackFixtureV22 localStackFixtureV22) : base(testFixture, localStackFixtureV22)
    {
    }
}

[Collection(nameof(LocalStackLegacyCollection))]
public sealed class DynamoDbLegacyScenario : BaseDynamoDbScenario
{
    public DynamoDbLegacyScenario(TestFixture testFixture, LocalStackLegacyFixture localStackLegacyFixture) : base(
        testFixture, localStackLegacyFixture, TestConstants.LegacyLocalStackConfig, true)
    {
    }
}