namespace LocalStack.Client.Tests.Mocks.MockServiceClients;

internal sealed class MockCredentials : BasicAWSCredentials
{
    public MockCredentials()
        : base("testkey", "testsecret")
    {
    }
}
