namespace LocalStack.Build.Models;

public record ProjMetadata(string DirectoryPath, string CsProjPath, IEnumerable<string> TargetFrameworks, string AssemblyName);