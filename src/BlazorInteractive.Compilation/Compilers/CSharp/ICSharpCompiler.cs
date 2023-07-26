namespace BlazorInteractive.Compilation;

public interface ICSharpCompiler
{
    Results.CSharpCompilationResult Compile(string sourceCode, string assemblyName, ReferenceCollection references, long languageVersion);
}