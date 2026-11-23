using BlazorInteractive.Compilation.Results;
using Microsoft.CodeAnalysis.CSharp;

namespace BlazorInteractive.Compilation;

public class CodeCompiler : ICompiler
{
    private readonly IReferenceResolver _referenceResolver;
    private readonly ICSharpCompiler _cSharpCompiler;
    private readonly IAssemblyLoader _assemblyLoader;
    private readonly IILEmitter _ilEmitter;

    public CodeCompiler(IReferenceResolver referenceResolver, ICSharpCompiler cSharpCompiler, IAssemblyLoader assemblyLoader, IILEmitter ilEmitter)
    {
        _referenceResolver = referenceResolver;
        _cSharpCompiler =  cSharpCompiler;
        _assemblyLoader = assemblyLoader;
        _ilEmitter = ilEmitter;
    }

    public async Task<CompilationResult> CompileAsync(string sourceCode, ICollection<string>? imports, LanguageVersion languageVersion, CancellationToken cancellationToken = default)
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

                    var compiler = _cSharpCompiler.Compile(sourceCode, assemblyName, refs, languageVersion);
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

    public async Task<ILEmitResult> EmitILAsync(string sourceCode, ICollection<string>? imports, LanguageVersion languageVersion, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(sourceCode))
        {
            return new Failure($"{nameof(sourceCode)} cannot be null or empty");
        }

        if (imports is null || !imports.Any())
        {
            return new Failure($"{nameof(imports)} cannot be null or empty");
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return new Cancelled();
        }

        try
        {
            var references = await _referenceResolver.ResolveAsync(imports, cancellationToken);

            return references.Match(
                refs =>
                {
                    var assemblyName = Path.GetRandomFileName();

                    var compiler = _cSharpCompiler.Compile(sourceCode, assemblyName, refs, languageVersion);
                    return compiler.Match(
                        compilation => _ilEmitter.Emit(compilation),
                        failure => (ILEmitResult)failure
                    );
                },
                failure => (ILEmitResult)failure,
                cancelled => (ILEmitResult)new Cancelled()
            );
        }
        catch (Exception ex)
        {
            return new Failure(ex, ex.Message);
        }
    }
}
