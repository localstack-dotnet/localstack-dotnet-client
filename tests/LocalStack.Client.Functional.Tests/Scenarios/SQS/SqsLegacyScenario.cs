namespace LocalStack.Client.Functional.Tests.Scenarios.SQS;

[Collection(nameof(LocalStackLegacyCollection))]
public class SqsLegacyScenario : SqsScenario
{
    public SqsLegacyScenario(TestFixture testFixture)
        : base(testFixture, TestConstants.LegacyLocalStackConfig, true)
    {
    }
}