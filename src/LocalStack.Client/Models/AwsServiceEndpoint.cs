namespace LocalStack.Client.Models;

public record AwsServiceEndpoint(string ServiceId, string CliName, AwsServiceEnum AwsServiceEnum, int Port, string Host, string ServiceUrl)
{
    public override string ToString()
    {
        return $"{ServiceId} - {ServiceUrl}";
    }
}