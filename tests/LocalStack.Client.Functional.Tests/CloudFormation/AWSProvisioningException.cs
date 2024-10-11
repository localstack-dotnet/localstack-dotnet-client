namespace LocalStack.Client.Functional.Tests.CloudFormation;

public class AwsProvisioningException : Exception
{
    public AwsProvisioningException() : base()
    {
    }

    public AwsProvisioningException(string message) : base(message)
    {
    }

    public AwsProvisioningException(string message, Exception innerException) : base(message, innerException)
    {
    }
}