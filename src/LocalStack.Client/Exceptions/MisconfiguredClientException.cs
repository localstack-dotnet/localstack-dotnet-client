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
#if NET8_0_OR_GREATER
#pragma warning disable S1133, MA0070, CA1041
    [Obsolete(DiagnosticId = "SYSLIB0051")] // add this attribute to the serialization ctor
#pragma warning restore MA0070, S1133, CA1041
#endif
    protected MisconfiguredClientException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
    {
    }
}