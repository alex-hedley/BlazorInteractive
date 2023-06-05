using System.Reflection;
using BlazorInteractive.Compilation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.Logging;

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
        EmitResult result = compilation.Value.Emit(ms);
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
    }
}
