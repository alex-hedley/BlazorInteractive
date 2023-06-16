using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;

namespace BlazorInteractive.Compilation;

public class VisualBasicCompilationBuilder : IVisualBasicCompilationBuilder
{
    public IVisualBasicCompilation Create(string? assemblyName, IEnumerable<SyntaxTree>? syntaxTrees = null, IEnumerable<IReference>? references = null, VisualBasicCompilationOptions? options = null)
    {
        return new VisualBasicCompilationWrapper(VisualBasicCompilation.Create(assemblyName, syntaxTrees, references?.Select(r => r.Value), options));
    }
}