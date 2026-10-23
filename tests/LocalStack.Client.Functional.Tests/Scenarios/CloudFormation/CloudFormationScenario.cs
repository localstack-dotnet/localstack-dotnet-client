#pragma warning disable MA0048 // File name must match type name - disabled because of readability

namespace LocalStack.Client.Functional.Tests.Scenarios.CloudFormation;

[Collection(nameof(LocalStackCollectionV414))]
public sealed class CloudFormationScenarioV414 : BaseCloudFormationScenario
{
    public CloudFormationScenarioV414(TestFixture testFixture, LocalStackFixtureV414 localStackFixtureV414) : base(testFixture, localStackFixtureV414)
    {
    }
}

[Collection(nameof(LocalStackCollectionCurrent))]
[Trait("LocalStackAuth", "Required")]
public sealed class CloudFormationScenarioCurrent : BaseCloudFormationScenario
{
    public CloudFormationScenarioCurrent(TestFixture testFixture, LocalStackFixtureCurrent localStackFixtureCurrent) : base(testFixture, localStackFixtureCurrent)
    {
    }
}