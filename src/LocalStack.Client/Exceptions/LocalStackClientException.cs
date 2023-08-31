namespace LocalStack.Client.Exceptions;

[Serializable]
public abstract class LocalStackClientException : Exception
{
    /// <summary>
    /// Construct instance of ConfigurationException
    /// </summary>
    /// <param name="message">The error message.</param>
    protected LocalStackClientException(string message) : base(message)
    {
    }

    /// <summary>
    /// Construct instance of ConfigurationException
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="exception">Original exception.</param>
    protected LocalStackClientException(string message, Exception exception) : base(message, exception)
    {
    }

    /// <inheritdoc />
    protected LocalStackClientException()
    {
    }

    /// <summary>
    /// Serialization constructor.
    /// </summary>
    /// <param name="serializationInfo">The information to use when serializing the exception.</param>
    /// <param name="streamingContext">The context for the serialization.</param>
    protected LocalStackClientException(SerializationInfo serializationInfo, StreamingContext streamingContext)
        : base(serializationInfo, streamingContext)
    {
    }
}