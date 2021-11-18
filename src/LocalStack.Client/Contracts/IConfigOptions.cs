namespace LocalStack.Client.Contracts;

public interface IConfigOptions
{
    string LocalStackHost { get; }

    bool UseSsl { get; }

    bool UseLegacyPorts { get; }

    int EdgePort { get; }
}