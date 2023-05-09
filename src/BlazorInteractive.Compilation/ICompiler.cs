
namespace BlazorInteractive.Compilation;

public interface ICompiler
{
    Task<string> CompileAsync(string sourceCode, IEnumerable<string> imports, CancellationToken cancellationToken = default);
}
