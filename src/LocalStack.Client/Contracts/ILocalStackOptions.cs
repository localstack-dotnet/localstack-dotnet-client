using LocalStack.Client.Options;

namespace LocalStack.Client.Contracts;

public interface ILocalStackOptions
{
    bool UseLocalStack { get; }

    SessionOptions Session { get; }

    ConfigOptions Config { get; }
}