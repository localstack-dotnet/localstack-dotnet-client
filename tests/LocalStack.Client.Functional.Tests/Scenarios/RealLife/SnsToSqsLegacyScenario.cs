namespace LocalStack.Client.Functional.Tests.Scenarios.RealLife;

[Collection(nameof(LocalStackLegacyCollection))]
public class SnsToSqsLegacyScenario : SnsToSqsScenario
{
    public SnsToSqsLegacyScenario(TestFixture testFixture) 
        : base(testFixture, TestConstants.LegacyLocalStackConfig, true)
    {
    }

    public override Task Should_Create_A_SNS_Topic_And_SQS_Queue_Then_Subscribe_To_The_Topic_Using_SQS_Then_Publish_A_Message_To_Topic_And_Read_It_From_The_Queue()
    {
        return Task.CompletedTask;
    }
}