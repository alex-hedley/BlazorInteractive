
namespace BlazorInteractive.Compilation;

public interface ICompiler
{
    Task<CompilationResult> CompileAsync(string sourceCode, IEnumerable<string> imports, CancellationToken cancellationToken = default);
}
