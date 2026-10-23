#pragma warning disable MA0048 // File name must match type name - disabled because of readability

namespace LocalStack.Client.Functional.Tests.Fixtures;

[CollectionDefinition(nameof(LocalStackCollectionV414))]
public class LocalStackCollectionV414 : ICollectionFixture<LocalStackFixtureV414>, ICollectionFixture<TestFixture>;

[CollectionDefinition(nameof(LocalStackCollectionCurrent))]
public class LocalStackCollectionCurrent : ICollectionFixture<LocalStackFixtureCurrent>, ICollectionFixture<TestFixture>;