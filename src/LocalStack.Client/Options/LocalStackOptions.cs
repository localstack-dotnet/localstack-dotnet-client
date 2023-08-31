namespace LocalStack.Client.Options;

public class LocalStackOptions : ILocalStackOptions
{
    public LocalStackOptions()
    {
        UseLocalStack = false;
        Session = new SessionOptions();
        Config = new ConfigOptions();
    }

    public LocalStackOptions(bool useLocalStack, SessionOptions sessionOptions, ConfigOptions configOptions)
    {
        UseLocalStack = useLocalStack;
        Session = sessionOptions;
        Config = configOptions;
    }

    public bool UseLocalStack { get; private set; }

    public SessionOptions Session { get; private set; }

    public ConfigOptions Config { get; private set; }
}