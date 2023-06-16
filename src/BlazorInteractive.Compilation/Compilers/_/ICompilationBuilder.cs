using Microsoft.CodeAnalysis;

namespace BlazorInteractive.Compilation;

public interface ICompilationBuilder
{
    Microsoft.CodeAnalysis.Compilation Create(string? assemblyName, IEnumerable<SyntaxTree>? syntaxTrees = null, IEnumerable<IReference>? references = null, CompilationOptions? options = null);
}