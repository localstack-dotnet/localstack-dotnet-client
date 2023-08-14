namespace LocalStack.Client.Functional.Tests.Scenarios.RealLife;

[Collection(nameof(LocalStackCollectionV131))]
public sealed class SnsToSqsScenarioV131 : BaseRealLife
{
    public SnsToSqsScenarioV131(TestFixture testFixture, LocalStackFixtureV131 localStackFixtureV131) : base(testFixture, localStackFixtureV131)
    {
    }
}

[Collection(nameof(LocalStackCollectionV20))]
public sealed class SnsToSqsScenarioV20 : BaseRealLife
{
    public SnsToSqsScenarioV20(TestFixture testFixture, LocalStackFixtureV20 localStackFixtureV20) : base(testFixture, localStackFixtureV20)
    {
    }
}

// [Collection(nameof(LocalStackCollectionV22))]
// public sealed class SnsToSqsScenarioV22 : BaseRealLife
// {
//     public SnsToSqsScenarioV22(TestFixture testFixture, LocalStackFixtureV22 localStackFixtureV22) : base(testFixture, localStackFixtureV22)
//     {
//     }
// }

[Collection(nameof(LocalStackLegacyCollection))]
public sealed class SnsToSqsLegacyScenario : BaseRealLife
{
    public SnsToSqsLegacyScenario(TestFixture testFixture, LocalStackLegacyFixture localStackFixtureV22) : base(
        testFixture, localStackFixtureV22, TestConstants.LegacyLocalStackConfig, true)
    {
    }

    public override Task Should_Create_A_SNS_Topic_And_SQS_Queue_Then_Subscribe_To_The_Topic_Using_SQS_Then_Publish_A_Message_To_Topic_And_Read_It_From_The_Queue()
    {
        return Task.CompletedTask;
    }
}