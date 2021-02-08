using System;

using Amazon.Runtime;

namespace LocalStack.Client.Tests.Mocks.MockServiceClients
{
    public interface IMockAmazonService : IDisposable, IAmazonService
    {

    }
}