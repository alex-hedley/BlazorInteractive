using System.Collections.ObjectModel;

namespace BlazorInteractive.Compilation;

public interface ICSharpCompiler
{
    Results.CSharpCompilationResult Compile(string sourceCode, string assemblyName, ReadOnlyCollection<IReference> references);
}