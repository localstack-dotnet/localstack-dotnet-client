namespace LocalStack.Client.Functional.Tests.Scenarios.SNS;

[Collection(nameof(LocalStackLegacyCollection))]
public class SnsLegacyScenario : SnsScenario
{
    public SnsLegacyScenario(TestFixture testFixture) 
        : base(testFixture, TestConstants.LegacyLocalStackConfig, true)
    {
    }

    public override Task Multi_Region_Tests(string systemName)
    {
        return Task.CompletedTask;
    }
}