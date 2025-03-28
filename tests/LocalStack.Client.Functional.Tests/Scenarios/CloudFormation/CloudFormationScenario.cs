#pragma warning disable MA0048 // File name must match type name - disabled because of readability

namespace LocalStack.Client.Functional.Tests.Scenarios.CloudFormation;

[Collection(nameof(LocalStackCollectionV37))]
public sealed class CloudFormationScenarioV37 : BaseCloudFormationScenario
{
    public CloudFormationScenarioV37(TestFixture testFixture, LocalStackFixtureV37 localStackFixtureV37) : base(testFixture, localStackFixtureV37)
    {
    }
}

[Collection(nameof(LocalStackCollectionV43))]
public sealed class CloudFormationScenarioV43 : BaseCloudFormationScenario
{
    public CloudFormationScenarioV43(TestFixture testFixture, LocalStackFixtureV43 localStackFixtureV43) : base(testFixture, localStackFixtureV43)
    {
    }
}