using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using BlazorInteractive.Compilation.Results;
using AssemblyLoaderResult = BlazorInteractive.Compilation.Results.AssemblyLoaderResult;

namespace BlazorInteractive.Compilation;

public class AssemblyLoader : IAssemblyLoader
{
    private readonly ILogger _logger;

    public AssemblyLoader(ILogger<AssemblyLoader> logger)
    {
        _logger = logger;
    }

    public AssemblyLoaderResult Load(ICSharpCompilation compilation)
    {
        using var ms = new MemoryStream();
        var result = compilation.Value?.Emit(ms);
        if (result is { Success: false })
        {
            var failures = result.Diagnostics.Where(diagnostic =>
                diagnostic.IsWarningAsError ||
                diagnostic.Severity == DiagnosticSeverity.Error)
                .ToList();

            foreach (var diagnostic in failures)
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
    }
}
