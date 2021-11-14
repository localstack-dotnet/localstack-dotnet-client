using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalStack.Build
{
    public class TestProjMetadata
    {
        public TestProjMetadata(string directoryPath, string csProjPath, string[] targetFrameworks, string assemblyName)
            => (DirectoryPath, CsProjPath, TargetFrameworks, AssemblyName) = (directoryPath, csProjPath, targetFrameworks, assemblyName);

        public string DirectoryPath { get; }

        public string CsProjPath { get; }

        public string AssemblyName { get; set; }

        public string[] TargetFrameworks { get; }
    }
}
