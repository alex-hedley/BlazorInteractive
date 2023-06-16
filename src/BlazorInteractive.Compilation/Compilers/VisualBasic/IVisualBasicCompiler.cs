using System.Collections.ObjectModel;
using Microsoft.CodeAnalysis.VisualBasic;

namespace BlazorInteractive.Compilation;

public interface IVisualBasicCompiler
{
    Results.VisualBasicCompilationResult Compile(string sourceCode, string assemblyName, ReferenceCollection references, LanguageVersion languageVersion);
}