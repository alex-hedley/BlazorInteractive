using Microsoft.CodeAnalysis.CSharp;

namespace BlazorInteractive.Compilation;

public interface _ICompiler
{
    Results.CompilationResult CompileAsync(string sourceCode, string assemblyName, ReferenceCollection references, long languageVersion, CancellationToken cancellationToken = default);
}