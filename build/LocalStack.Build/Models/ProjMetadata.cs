#pragma warning disable CA1515 // Consider making public types internal

namespace LocalStack.Build.Models;

public record ProjMetadata(string DirectoryPath, string CsProjPath, IEnumerable<string> TargetFrameworks, string AssemblyName);