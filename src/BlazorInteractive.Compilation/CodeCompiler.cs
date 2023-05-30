using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BlazorInteractive.Compilation;

public class CodeCompiler : ICompiler
{
    private readonly ILogger _logger;
    private readonly IReferenceResolver _referenceResolver;

    public CodeCompiler(ILogger<CodeCompiler> logger, IReferenceResolver referenceResolver)
    {
        _referenceResolver = referenceResolver;
        _logger = logger;
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

        try
        {
            var references = await _referenceResolver.ResolveAsync(imports, cancellationToken);

            return await references.Match<Task<CompilationResult>>(
                async refs =>
                {
                    var assemblyName = Path.GetRandomFileName();
                    var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Default);
                    var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(sourceCode, options);


                    CSharpCompilation compilation = CSharpCompilation.Create(
                        assemblyName,
                        syntaxTrees: new[] { parsedSyntaxTree },
                        references: refs,
                        options: new CSharpCompilationOptions(
                            OutputKind.DynamicallyLinkedLibrary,
                            concurrentBuild: false,
                            optimizationLevel: OptimizationLevel.Debug
                        )
                    );

                    using var ms = new MemoryStream();
                    EmitResult result = compilation.Emit(ms);
                    if (!result.Success)
                    {
                        IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                            diagnostic.IsWarningAsError ||
                            diagnostic.Severity == DiagnosticSeverity.Error);

                        foreach (Diagnostic diagnostic in failures)
                        {
                            _logger.LogError("{Id}: {Message}", diagnostic.Id, diagnostic.GetMessage());
                        }
                        return new Failure(string.Join(Environment.NewLine, failures));
                    }
                    else
                    {
                        ms.Seek(0, SeekOrigin.Begin);
                        var assembly = Assembly.Load(ms.ToArray());
                        return assembly;
                    }
                },
                async failure => await Task.FromResult(failure),
                async cancelled => await Task.FromResult(cancelled));
        }
        catch (OperationCanceledException)
        {
            return new Cancelled();
        }
        catch (Exception ex)
        {
            return new Failure(ex, ex.Message);
        }
    }
}