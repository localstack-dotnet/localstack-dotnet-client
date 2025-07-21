#pragma warning disable MA0048 // File name must match type name - disabled because of readability

namespace LocalStack.Client.Functional.Tests.Scenarios.CloudFormation;

[Collection(nameof(LocalStackCollectionV37))]
public sealed class CloudFormationScenarioV37 : BaseCloudFormationScenario
{
    public CloudFormationScenarioV37(TestFixture testFixture, LocalStackFixtureV37 localStackFixtureV37) : base(testFixture, localStackFixtureV37)
    {
    }
}

[Collection(nameof(LocalStackCollectionV46))]
public sealed class CloudFormationScenarioV46 : BaseCloudFormationScenario
{
    public CloudFormationScenarioV46(TestFixture testFixture, LocalStackFixtureV46 localStackFixtureV46) : base(testFixture, localStackFixtureV46)
    {
    }
}