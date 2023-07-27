namespace BlazorInteractive.Compilation;

public interface IVisualBasicCompiler
{
    Results.VisualBasicCompilationResult Compile(string sourceCode, string assemblyName, ReferenceCollection references, long languageVersion);
}