using System.Collections.ObjectModel;

namespace BlazorInteractive.Compilation;

public interface ICSharpCompiler
{
    CSharpCompilationResult Compile(string sourceCode, string assemblyName, ReadOnlyCollection<IReference> references);
}