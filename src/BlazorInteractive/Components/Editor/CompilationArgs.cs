using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;

public class CompilationArgs
{
    public string SourceCode { get; set; }
    public CancellationToken CancellationToken { get; set; }
    public List<string> References { get; set; }
    public LanguageVersion LanguageVersion { get; set; }
    public List<ImmutableArray<byte>> PackageAssemblies { get; set; } = new();
}