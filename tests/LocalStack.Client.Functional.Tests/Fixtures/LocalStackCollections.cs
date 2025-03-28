﻿#pragma warning disable MA0048 // File name must match type name - disabled because of readability

namespace LocalStack.Client.Functional.Tests.Fixtures;

[CollectionDefinition(nameof(LocalStackCollectionV37))]
public class LocalStackCollectionV37 : ICollectionFixture<LocalStackFixtureV37>, ICollectionFixture<TestFixture>;

[CollectionDefinition(nameof(LocalStackCollectionV43))]
public class LocalStackCollectionV43 : ICollectionFixture<LocalStackFixtureV43>, ICollectionFixture<TestFixture>;