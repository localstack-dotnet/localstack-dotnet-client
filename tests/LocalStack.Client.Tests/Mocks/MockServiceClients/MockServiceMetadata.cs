using Amazon.Runtime.Internal;

using System.Collections.Generic;

namespace LocalStack.Client.Tests.Mocks.MockServiceClients
{
    public class MockServiceMetadata : IServiceMetadata
    {
        public const string MockServiceId = "Mock Amazon Service";

        public string ServiceId => MockServiceId;

        public IDictionary<string, string> OperationNameMapping { get; }
    }
}