#pragma warning disable MA0048 // File name must match type name - disabled because of readability

namespace LocalStack.Client.Functional.Tests.Scenarios.SQS;

[Collection(nameof(LocalStackCollectionV131))]
public sealed class SqsScenarioV131 : BaseSqsScenario
{
    public SqsScenarioV131(TestFixture testFixture, LocalStackFixtureV131 localStackFixtureV131) : base(testFixture, localStackFixtureV131)
    {
    }

    // Test disabled because of incompatibility between AWSSDK.SQS 3.7.300 and above and LocalStack v1 and v2 series
    public override Task AmazonSqsService_Should_Create_A_Queue_Async()
    {
        Assert.True(true);

        return Task.CompletedTask;
    }

    // Test disabled because of incompatibility between AWSSDK.SQS 3.7.300 and above and LocalStack v1 and v2 series
    public override Task AmazonSqsService_Should_Delete_A_Queue_Async()
    {
        Assert.True(true);

        return Task.CompletedTask;
    }

    // Test disabled because of incompatibility between AWSSDK.SQS 3.7.300 and above and LocalStack v1 and v2 series
    public override Task AmazonSqsService_Should_Send_A_Message_To_A_Queue_Async()
    {
        Assert.True(true);

        return Task.CompletedTask;
    }

    // Test disabled because of incompatibility between AWSSDK.SQS 3.7.300 and above and LocalStack v1 and v2 series
    public override Task AmazonSqsService_Should_Receive_Messages_From_A_Queue_Async()
    {
        Assert.True(true);

        return Task.CompletedTask;
    }
}

[Collection(nameof(LocalStackCollectionV23))]
public sealed class SqsScenarioV23 : BaseSqsScenario
{
    public SqsScenarioV23(TestFixture testFixture, LocalStackFixtureV23 localStackFixtureV23) : base(testFixture, localStackFixtureV23)
    {
    }

    // Test disabled because of incompatibility between AWSSDK.SQS 3.7.300 and above and LocalStack v1 and v2 series
    public override Task AmazonSqsService_Should_Create_A_Queue_Async()
    {
        Assert.True(true);

        return Task.CompletedTask;
    }

    // Test disabled because of incompatibility between AWSSDK.SQS 3.7.300 and above and LocalStack v1 and v2 series
    public override Task AmazonSqsService_Should_Delete_A_Queue_Async()
    {
        Assert.True(true);

        return Task.CompletedTask;
    }

    // Test disabled because of incompatibility between AWSSDK.SQS 3.7.300 and above and LocalStack v1 and v2 series
    public override Task AmazonSqsService_Should_Send_A_Message_To_A_Queue_Async()
    {
        Assert.True(true);

        return Task.CompletedTask;
    }

    // Test disabled because of incompatibility between AWSSDK.SQS 3.7.300 and above and LocalStack v1 and v2 series
    public override Task AmazonSqsService_Should_Receive_Messages_From_A_Queue_Async()
    {
        Assert.True(true);

        return Task.CompletedTask;
    }
}

[Collection(nameof(LocalStackCollectionV34))]
public sealed class SqsScenarioV34 : BaseSqsScenario
{
    public SqsScenarioV34(TestFixture testFixture, LocalStackFixtureV34 localStackFixtureV34) : base(testFixture, localStackFixtureV34)
    {
    }
}

[Collection(nameof(LocalStackCollectionV38))]
public sealed class SqsScenarioV38 : BaseSqsScenario
{
    public SqsScenarioV38(TestFixture testFixture, LocalStackFixtureV38 localStackFixtureV38) : base(testFixture, localStackFixtureV38)
    {
    }
}