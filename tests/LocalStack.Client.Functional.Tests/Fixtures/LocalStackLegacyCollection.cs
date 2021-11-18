namespace LocalStack.Client.Functional.Tests.Fixtures;

[CollectionDefinition(nameof(LocalStackLegacyCollection))]
public class LocalStackLegacyCollection : ICollectionFixture<LocalStackLegacyFixture>, ICollectionFixture<TestFixture>
{

}
