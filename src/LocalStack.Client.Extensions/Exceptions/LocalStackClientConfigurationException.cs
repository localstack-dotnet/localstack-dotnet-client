using System;

namespace LocalStack.Client.Extensions.Exceptions
{
    public class LocalStackClientConfigurationException : Exception
    {
        /// <summary>
        /// Construct instance of ConfigurationException
        /// </summary>
        /// <param name="message">The error message.</param>
        public LocalStackClientConfigurationException(string message) : base(message) { }

        /// <summary>
        /// Construct instance of ConfigurationException
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="exception">Original exception.</param>
        public LocalStackClientConfigurationException(string message, Exception exception) : base(message, exception) { }
    }
}
