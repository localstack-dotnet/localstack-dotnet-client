﻿namespace LocalStack.Client.Functional.Tests.Scenarios.SNS;

[Collection(nameof(LocalStackCollectionV131))]
public sealed class SnsScenarioV131 : BaseSnsScenario
{
    public SnsScenarioV131(TestFixture testFixture, LocalStackFixtureV131 localStackFixtureV131) : base(testFixture, localStackFixtureV131)
    {
    }
}

[Collection(nameof(LocalStackCollectionV20))]
public sealed class SnsScenarioV20 : BaseSnsScenario
{
    public SnsScenarioV20(TestFixture testFixture, LocalStackFixtureV20 localStackFixtureV20) : base(testFixture, localStackFixtureV20)
    {
    }
}

[Collection(nameof(LocalStackCollectionV22))]
public sealed class SnsScenarioV22 : BaseSnsScenario
{
    public SnsScenarioV22(TestFixture testFixture, LocalStackFixtureV22 localStackFixtureV22) : base(testFixture, localStackFixtureV22)
    {
    }
}

[Collection(nameof(LocalStackLegacyCollection))]
public sealed class SnsLegacyScenario : BaseSnsScenario
{
    public SnsLegacyScenario(TestFixture testFixture, LocalStackLegacyFixture localStackLegacyFixture) : base(
        testFixture, localStackLegacyFixture, TestConstants.LegacyLocalStackConfig, true)
    {
    }

    public override Task Multi_Region_Tests(string systemName)
    {
        return Task.CompletedTask;
    }
}