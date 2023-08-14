namespace LocalStack.Client.Functional.Tests.Fixtures;

[CollectionDefinition(nameof(LocalStackCollectionV131))]
public class LocalStackCollectionV131 : ICollectionFixture<LocalStackFixtureV131>, ICollectionFixture<TestFixture>
{
}

[CollectionDefinition(nameof(LocalStackCollectionV20))]
public class LocalStackCollectionV20 : ICollectionFixture<LocalStackFixtureV20>, ICollectionFixture<TestFixture>
{
}

[CollectionDefinition(nameof(LocalStackCollectionV22))]
public class LocalStackCollectionV22 : ICollectionFixture<LocalStackFixtureV22>, ICollectionFixture<TestFixture>
{
}

[CollectionDefinition(nameof(LocalStackLegacyCollection))]
public class LocalStackLegacyCollection : ICollectionFixture<LocalStackLegacyFixture>, ICollectionFixture<TestFixture>
{
}