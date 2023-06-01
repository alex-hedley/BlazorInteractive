using Microsoft.CodeAnalysis;
using System.Collections.ObjectModel;

using BlazorInteractive.Compilation;

public interface ICSharpCompiler
{
    CSharpCompilationResult Compile(string sourceCode, string assemblyName, ReadOnlyCollection<MetadataReference> references);
}