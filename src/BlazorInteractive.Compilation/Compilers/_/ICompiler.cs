namespace BlazorInteractive.Compilation;

public interface ICompiler
{
    Results.CompilationResult Compile(string sourceCode, string assemblyName, ReferenceCollection references, LanguageVersion languageVersion);
}