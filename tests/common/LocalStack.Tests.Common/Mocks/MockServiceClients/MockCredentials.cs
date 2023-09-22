namespace LocalStack.Tests.Common.Mocks.MockServiceClients;

internal sealed class MockCredentials : BasicAWSCredentials
{
    public MockCredentials()
        : base("testkey", "testsecret")
    {
    }
}
