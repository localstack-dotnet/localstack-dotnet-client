#pragma warning disable MA0048 // File name must match type name - disabled because of readability

namespace LocalStack.Client.Functional.Tests.Scenarios.CloudFormation;


[Collection(nameof(LocalStackCollectionV131))]
public class CloudFormationScenarioV131 : BaseCloudFormationScenario
{
    public CloudFormationScenarioV131(TestFixture testFixture, LocalStackFixtureV131 localStackFixtureV131) : base(testFixture, localStackFixtureV131)
    {
    }
}

[Collection(nameof(LocalStackCollectionV23))]
public sealed class CloudFormationScenarioV23 : BaseCloudFormationScenario
{
    public CloudFormationScenarioV23(TestFixture testFixture, LocalStackFixtureV23 localStackFixtureV23) : base(testFixture, localStackFixtureV23)
    {
    }
}

[Collection(nameof(LocalStackCollectionV34))]
public sealed class CloudFormationScenarioV34 : BaseCloudFormationScenario
{
    public CloudFormationScenarioV34(TestFixture testFixture, LocalStackFixtureV34 localStackFixtureV34) : base(testFixture, localStackFixtureV34)
    {
    }
}