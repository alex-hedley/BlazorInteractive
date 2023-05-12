using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace BlazorInteractive.Compilation;

public class ScriptCompiler : ICompiler
{
    public async Task<CompilationResult> CompileAsync(string sourceCode, IEnumerable<string> imports, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(sourceCode))
        {
            return new Failure($"{nameof(sourceCode)} cannot be null or empty");
        }

        if (imports is null || !imports.Any())
        {
            return new Failure($"{nameof(imports)} cannot be null or empty");
        }

        // // Grab any Console info.
        // var defaultWriter = Console.Out;
        // using var writer = new StringWriter();
        // Console.SetOut(writer);
        // var result = writer.ToString();
        // Console.SetOut(defaultWriter);

        try
        {
            object result = await CSharpScript.EvaluateAsync(sourceCode, ScriptOptions.Default.WithImports(imports), cancellationToken: cancellationToken);
            return (result is null) ? new Void() : new Success(result.ToString()!);
        }
        catch (CompilationErrorException cee)
        {
            return new Failure(cee, cee.Message);
        }
        catch (OperationCanceledException)
        {
            return new Cancelled();
        }
    }
}