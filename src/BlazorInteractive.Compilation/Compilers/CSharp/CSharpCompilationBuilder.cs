using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace BlazorInteractive.Compilation;

public class CSharpCompilationBuilder : ICSharpCompilationBuilder
{
    public ICSharpCompilation Create(string? assemblyName, IEnumerable<SyntaxTree>? syntaxTrees = null, IEnumerable<IReference>? references = null, CSharpCompilationOptions? options = null)
    {
        return new CSharpCompilationWrapper(CSharpCompilation.Create(assemblyName, syntaxTrees, references?.Select(r => r.Value), options));
    }
}