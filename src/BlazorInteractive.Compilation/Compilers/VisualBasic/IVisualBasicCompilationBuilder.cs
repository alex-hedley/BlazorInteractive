using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;

namespace BlazorInteractive.Compilation;

public interface IVisualBasicCompilationBuilder
{
    IVisualBasicCompilation Create(string? assemblyName, IEnumerable<SyntaxTree>? syntaxTrees = null, IEnumerable<IReference>? references = null, VisualBasicCompilationOptions? options = null);
}