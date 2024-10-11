namespace LocalStack.Client.Extensions.Exceptions;

[Serializable]
public class LocalStackClientConfigurationException : Exception
{
    /// <summary>
    /// Construct instance of ConfigurationException
    /// </summary>
    /// <param name="message">The error message.</param>
    public LocalStackClientConfigurationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Construct instance of ConfigurationException
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="exception">Original exception.</param>
    public LocalStackClientConfigurationException(string message, Exception exception) : base(message, exception)
    {
    }

    public LocalStackClientConfigurationException()
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
    protected LocalStackClientConfigurationException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
    {
    }
}