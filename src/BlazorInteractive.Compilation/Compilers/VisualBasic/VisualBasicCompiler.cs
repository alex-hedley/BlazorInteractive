using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;

using BlazorInteractive.Compilation.Extensions;
using BlazorInteractive.Compilation.Results;

namespace BlazorInteractive.Compilation;

public class VisualBasicCompiler : IVisualBasicCompiler
{
    public VisualBasicCompilationResult Compile(string sourceCode, string assemblyName, ReferenceCollection references, LanguageVersion languageVersion)
    {
        if (string.IsNullOrEmpty(sourceCode))
        {
            return new Failure($"{nameof(sourceCode)} cannot be null or empty");
        }

        if (string.IsNullOrEmpty(assemblyName))
        {
            return new Failure($"{nameof(assemblyName)} cannot be null or empty");
        }

        var sourceCodeWithUsings = references
            .ToUsings()
            .Append(sourceCode)
            .Join(Environment.NewLine);

        var parseOptions = VisualBasicParseOptions.Default.WithLanguageVersion(languageVersion);
        var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(sourceCodeWithUsings, parseOptions);

        var options = new VisualBasicCompilationOptions(
            OutputKind.DynamicallyLinkedLibrary,
            concurrentBuild: false,
            optimizationLevel: OptimizationLevel.Debug
        );

        IVisualBasicCompilationBuilder builder = new VisualBasicCompilationBuilder();
        var compilation = builder.Create(assemblyName, new[] { parsedSyntaxTree }, references, options);
        return new VisualBasicCompilationWrapper(compilation.Value);
    }
}