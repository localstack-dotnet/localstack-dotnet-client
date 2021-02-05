using System.Collections.Generic;
using System.Text;

namespace LocalStack.Client.Contracts
{
    public interface IConfigOptions
    {
        string LocalStackHost { get; }

        bool UseSsl { get; }

        bool UseLegacyPorts { get; }

        int EdgePort { get; }
    }
}
