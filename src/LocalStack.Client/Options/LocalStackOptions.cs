namespace LocalStack.Client.Options;

public class LocalStackOptions : ILocalStackOptions
{
    public LocalStackOptions()
    {
    }

    public LocalStackOptions(bool useLocalStack, SessionOptions sessionOptions, ConfigOptions configOptions)
    {
        UseLocalStack = useLocalStack;
        Session = sessionOptions;
        Config = configOptions;
    }

    public bool UseLocalStack { get; private set; } = false;

    public SessionOptions Session { get; private set; } = new SessionOptions();

    public ConfigOptions Config { get; private set; } = new ConfigOptions();
}