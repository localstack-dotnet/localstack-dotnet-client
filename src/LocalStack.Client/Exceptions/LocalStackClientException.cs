namespace LocalStack.Client.Exceptions;

[Serializable]
#if NETFRAMEWORK
public class LocalStackClientException : Exception, System.Runtime.InteropServices._Exception
#else
public class LocalStackClientException : Exception
#endif
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
#if NET8_0_OR_GREATER
#pragma warning disable S1133, MA0070, CA1041
    [Obsolete(DiagnosticId = "SYSLIB0051")] // add this attribute to the serialization ctor
#pragma warning restore MA0070, S1133, CA1041
#endif
    protected LocalStackClientException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
    {
    }
}