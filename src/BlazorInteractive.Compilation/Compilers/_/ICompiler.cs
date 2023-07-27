namespace BlazorInteractive.Compilation;

public interface ICompiler
{
    Task<Results.CompilationResult> CompileAsync(string sourceCode, ICollection<string>? imports, string language, long languageVersion, CancellationToken cancellationToken = default);
}
