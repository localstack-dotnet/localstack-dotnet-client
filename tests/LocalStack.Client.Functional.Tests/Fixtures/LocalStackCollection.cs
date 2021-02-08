using Xunit;

namespace LocalStack.Client.Functional.Tests.Fixtures
{
    [CollectionDefinition(nameof(LocalStackCollection))]
    public class LocalStackCollection : ICollectionFixture<LocalStackFixture>, ICollectionFixture<TestFixture>
    {

    }
}