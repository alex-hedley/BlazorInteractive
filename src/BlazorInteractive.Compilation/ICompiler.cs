
namespace BlazorInteractive.Compilation;

public interface ICompiler
{
    Task<CompilationResult> CompileAsync(string sourceCode, ICollection<string>? imports, CancellationToken cancellationToken = default);
}
