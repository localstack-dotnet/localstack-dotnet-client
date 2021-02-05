using System;

using Amazon.Runtime;

namespace LocalStack.Client.Tests.Mocks.MockServiceClients
{
    public interface IMockAmazonServiceWithServiceMetadata : IDisposable, IAmazonService
    {

    }
}