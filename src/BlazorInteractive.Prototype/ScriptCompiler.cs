using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

public class ScriptCompiler : ICompiler
{
    public async Task<string> Compile(string sourceCode, IEnumerable<string> imports)
    {
        object result = await CSharpScript.EvaluateAsync(sourceCode, ScriptOptions.Default.WithImports(imports));
        return (result is null) ? string.Empty : result.ToString()!;
    }
}