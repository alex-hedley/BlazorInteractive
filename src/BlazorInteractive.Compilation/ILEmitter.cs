using BlazorInteractive.Compilation.Results;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace BlazorInteractive.Compilation;

public class ILEmitter : IILEmitter
{
    private readonly ILogger _logger;

    public ILEmitter(ILogger<ILEmitter> logger)
    {
        _logger = logger;
    }

    public ILEmitResult Emit(ICSharpCompilation compilation)
    {
        if (compilation.Value is null)
        {
            return new Failure("Compilation is not available");
        }

        using var ms = new MemoryStream();
        var result = compilation.Value.Emit(ms);
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

        ms.Seek(0, SeekOrigin.Begin);
        var module = new PEFile("compiled", ms);
        var output = new PlainTextOutput();
        var disassembler = new ReflectionDisassembler(output, CancellationToken.None);
        disassembler.WriteModuleContents(module);
        return output.ToString();
    }
}
