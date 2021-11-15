namespace LocalStack.Build.Models;

public record ProjMetadata(string DirectoryPath, string CsProjPath, IEnumerable TargetFrameworks, string AssemblyName);