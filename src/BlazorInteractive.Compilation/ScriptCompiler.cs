using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace BlazorInteractive.Compilation;

public class ScriptCompiler : ICompiler
{
    public async Task<CompilationResult> CompileAsync(string sourceCode, IEnumerable<string> imports, CancellationToken cancellationToken = default)
    {
        if (sourceCode is null)
        {
            return new Failure();
        }
        
        // if (!imports?.Any())
        if (imports is null || imports.Count() == 0)
        {
            return new Failure();
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
            return new Failure();
        }
    }
}