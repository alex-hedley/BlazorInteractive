using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace BlazorInteractive.Compilation;

public interface ICSharpCompilationBuilder
{
    ICSharpCompilation Create(string? assemblyName, IEnumerable<SyntaxTree>? syntaxTrees = null, IEnumerable<IReference>? references = null, CSharpCompilationOptions? options = null);
}