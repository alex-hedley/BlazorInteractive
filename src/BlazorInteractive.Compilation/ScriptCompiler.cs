using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace BlazorInteractive.Compilation;

public class ScriptCompiler : ICompiler
{
    public async Task<string> CompileAsync(string sourceCode, IEnumerable<string> imports, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(sourceCode);
        ArgumentNullException.ThrowIfNull(imports);

        // // Grab any Console info.
        // var defaultWriter = Console.Out;
        // using var writer = new StringWriter();
        // Console.SetOut(writer);
        // var result = writer.ToString();
        // Console.SetOut(defaultWriter);

        object result = await CSharpScript.EvaluateAsync(sourceCode, ScriptOptions.Default.WithImports(imports), cancellationToken: cancellationToken);
        return (result is null) ? string.Empty : result.ToString()!;
    }
}