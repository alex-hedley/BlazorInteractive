using Microsoft.CodeAnalysis;

namespace BlazorInteractive.Compilation;

public interface ICSharpCompilationBuilder
{
    ICSharpCompilation Create(string? assemblyName, IEnumerable<SyntaxTree>? syntaxTrees = null, IEnumerable<IReference>? references = null);
}