using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Amazon.Runtime;

namespace LocalStack.Client.Tests.Mocks.MockServiceClients
{
    internal class MockCredentials :BasicAWSCredentials
    {
        public MockCredentials() 
            : base("testkey", "testsecret")
        {
        }
    }
}
