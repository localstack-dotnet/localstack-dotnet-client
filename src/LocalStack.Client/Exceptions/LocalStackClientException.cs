namespace LocalStack.Client.Exceptions;

public abstract class LocalStackClientException : Exception
{
    /// <summary>
    /// Construct instance of ConfigurationException
    /// </summary>
    /// <param name="message">The error message.</param>
    protected LocalStackClientException(string message) : base(message) { }

    /// <summary>
    /// Construct instance of ConfigurationException
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="exception">Original exception.</param>
    protected LocalStackClientException(string message, Exception exception) : base(message, exception) { }
}

public class NotSupportedClientException : LocalStackClientException
{
    public NotSupportedClientException(string message) 
        : base(message)
    {
    }

    public NotSupportedClientException(string message, Exception exception) 
        : base(message, exception)
    {
    }
}

public class MisconfiguredClientException : LocalStackClientException
{
    public MisconfiguredClientException(string message) 
        : base(message)
    {
    }

    public MisconfiguredClientException(string message, Exception exception) 
        : base(message, exception)
    {
    }
}