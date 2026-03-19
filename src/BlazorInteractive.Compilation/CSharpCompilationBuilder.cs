using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace BlazorInteractive.Compilation;

public class CSharpCompilationBuilder : ICSharpCompilationBuilder
{
    public ICSharpCompilation Create(string? assemblyName, IEnumerable<SyntaxTree>? syntaxTrees = null, IEnumerable<IReference>? references = null, CSharpCompilationOptions? options = null)
    {
        // CreateScriptCompilation accepts a single syntax tree; the compilation pipeline
        // always supplies exactly one tree (user code + prepended usings).
        return new CSharpCompilationWrapper(
            CSharpCompilation.CreateScriptCompilation(
                assemblyName ?? string.Empty,
                syntaxTrees?.FirstOrDefault(),
                references?.Select(r => r.Value),
                options));
    }
}