namespace LocalStack.Client.Functional.Tests.Scenarios.S3;

[Collection(nameof(LocalStackLegacyCollection))]
public class S3LegacyScenario : S3Scenario
{
    public S3LegacyScenario(TestFixture testFixture)
        : base(testFixture, TestConstants.LegacyLocalStackConfig)
    {
    }
}
