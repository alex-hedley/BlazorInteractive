using Microsoft.CodeAnalysis;

namespace BlazorInteractive.Compilation;

public class CodeCompiler : ICompiler
{
    private readonly IReferenceResolver _referenceResolver;
    private readonly ICSharpCompiler _cSharpCompiler;
    private readonly IAssemblyLoader _assemblyLoader;

    public CodeCompiler(IReferenceResolver referenceResolver, ICSharpCompiler cSharpCompiler, IAssemblyLoader assemblyLoader)
    {
        _referenceResolver = referenceResolver;
        _cSharpCompiler =  cSharpCompiler;
        _assemblyLoader = assemblyLoader;
    }

    public async Task<CompilationResult> CompileAsync(string sourceCode, ICollection<string>? imports, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(sourceCode))
        {
            return new Failure($"{nameof(sourceCode)} cannot be null or empty");
        }

        if (imports is null || !imports.Any())
        {
            return new Failure($"{nameof(imports)} cannot be null or empty");
        }

        if (cancellationToken.IsCancellationRequested) {
            return new Cancelled();
        }

        try
        {
            var references = await _referenceResolver.ResolveAsync(imports, cancellationToken);

            return references.Match(
                refs =>
                {
                    var assemblyName = Path.GetRandomFileName();

                    var compiler = _cSharpCompiler.Compile(sourceCode, assemblyName, refs);
                    return compiler.Match(
                        compilation =>
                        {
                            return _assemblyLoader.Load(compilation).Match<CompilationResult>(
                                assembly => assembly,
                                failure => failure
                            );
                        },
                        failure => failure
                    );
                },
                failure => failure,
                cancelled => cancelled
            );
        }
        catch (Exception ex)
        {
            return new Failure(ex, ex.Message);
        }
    }
}