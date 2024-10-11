#pragma warning disable MA0048 // File name must match type name - disabled because of readability

namespace LocalStack.Client.Functional.Tests.Fixtures;

[CollectionDefinition(nameof(LocalStackCollectionV131))]
public class LocalStackCollectionV131 : ICollectionFixture<LocalStackFixtureV131>, ICollectionFixture<TestFixture>;

[CollectionDefinition(nameof(LocalStackCollectionV23))]
public class LocalStackCollectionV23 : ICollectionFixture<LocalStackFixtureV23>, ICollectionFixture<TestFixture>;

[CollectionDefinition(nameof(LocalStackCollectionV34))]
public class LocalStackCollectionV34 : ICollectionFixture<LocalStackFixtureV34>, ICollectionFixture<TestFixture>;

[CollectionDefinition(nameof(LocalStackCollectionV38))]
public class LocalStackCollectionV38 : ICollectionFixture<LocalStackFixtureV38>, ICollectionFixture<TestFixture>;