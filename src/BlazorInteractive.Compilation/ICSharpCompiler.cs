using System.Collections.ObjectModel;
using Microsoft.CodeAnalysis.CSharp;

namespace BlazorInteractive.Compilation;

public interface ICSharpCompiler
{
    Results.CSharpCompilationResult Compile(string sourceCode, string assemblyName, ReferenceCollection references, LanguageVersion languageVersion);
}