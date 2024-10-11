#pragma warning disable MA0048 // File name must match type name - disabled because of readability

namespace LocalStack.Client.Functional.Tests.Scenarios.RealLife;

[Collection(nameof(LocalStackCollectionV131))]
public sealed class SnsToSqsScenarioV131 : BaseRealLife
{
    public SnsToSqsScenarioV131(TestFixture testFixture, LocalStackFixtureV131 localStackFixtureV131) : base(testFixture, localStackFixtureV131)
    {
    }

    // Test disabled because of incompatibility between AWSSDK.SQS 3.7.300 and above and LocalStack v1 and v2 series
    public override Task Should_Create_A_SNS_Topic_And_SQS_Queue_Then_Subscribe_To_The_Topic_Using_SQS_Then_Publish_A_Message_To_Topic_And_Read_It_From_The_Queue_Async()
    {
        Assert.True(true);

        return Task.CompletedTask;
    }
}

[Collection(nameof(LocalStackCollectionV23))]
public sealed class SnsToSqsScenarioV23 : BaseRealLife
{
    public SnsToSqsScenarioV23(TestFixture testFixture, LocalStackFixtureV23 localStackFixtureV23) : base(testFixture, localStackFixtureV23)
    {
    }

    // Test disabled because of incompatibility between AWSSDK.SQS 3.7.300 and above and LocalStack v1 and v2 series
    public override Task Should_Create_A_SNS_Topic_And_SQS_Queue_Then_Subscribe_To_The_Topic_Using_SQS_Then_Publish_A_Message_To_Topic_And_Read_It_From_The_Queue_Async()
    {
        Assert.True(true);

        return Task.CompletedTask;
    }
}

[Collection(nameof(LocalStackCollectionV34))]
public sealed class SnsToSqsScenarioV34 : BaseRealLife
{
    public SnsToSqsScenarioV34(TestFixture testFixture, LocalStackFixtureV34 localStackFixtureV34) : base(testFixture, localStackFixtureV34)
    {
    }
}

[Collection(nameof(LocalStackCollectionV38))]
public sealed class SnsToSqsScenarioV38 : BaseRealLife
{
    public SnsToSqsScenarioV38(TestFixture testFixture, LocalStackFixtureV38 localStackFixtureV38) : base(testFixture, localStackFixtureV38)
    {
    }
}