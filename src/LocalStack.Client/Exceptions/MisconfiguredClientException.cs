namespace LocalStack.Client.Exceptions;

[Serializable]
public class MisconfiguredClientException : LocalStackClientException
{
    /// <inheritdoc />
    public MisconfiguredClientException(string message) : base(message)
    {
    }

    /// <inheritdoc />
    public MisconfiguredClientException(string message, Exception exception) : base(message, exception)
    {
    }

    /// <inheritdoc />
    public MisconfiguredClientException()
    {
    }

    /// <inheritdoc />
    protected MisconfiguredClientException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
    {
    }
}