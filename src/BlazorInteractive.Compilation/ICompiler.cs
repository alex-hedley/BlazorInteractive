
using Microsoft.CodeAnalysis.CSharp;

namespace BlazorInteractive.Compilation;

public interface ICompiler
{
    Task<Results.CompilationResult> CompileAsync(string sourceCode, ICollection<string>? imports, LanguageVersion languageVersion, CancellationToken cancellationToken = default);
}
