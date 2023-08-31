namespace LocalStack.Client.Models;

public record AwsServiceEndpoint(string ServiceId, string CliName, AwsService AwsService, int Port, string Host, Uri ServiceUrl)
{
    public override string ToString()
    {
        return $"{ServiceId} - {ServiceUrl}";
    }
}