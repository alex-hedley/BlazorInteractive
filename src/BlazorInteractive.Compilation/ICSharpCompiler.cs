using Microsoft.CodeAnalysis;
using System.Collections.ObjectModel;

using BlazorInteractive.Compilation;

namespace BlazorInteractive.Compilation;

public interface ICSharpCompiler
{
    CSharpCompilationResult Compile(string sourceCode, string assemblyName, ReadOnlyCollection<IReference> references);
}