namespace LocalStack.Client.Exceptions;

[Serializable]
public class NotSupportedClientException : LocalStackClientException
{
    /// <inheritdoc />
    public NotSupportedClientException(string message) : base(message)
    {
    }

    /// <inheritdoc />
    public NotSupportedClientException(string message, Exception exception) : base(message, exception)
    {
    }

    public NotSupportedClientException()
    {
    }

    /// <inheritdoc />
    protected NotSupportedClientException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
    {
    }
}