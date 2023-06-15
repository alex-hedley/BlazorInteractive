using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using BlazorInteractive.Compilation.Extensions;
using BlazorInteractive.Compilation.Results;
using CSharpCompilationResult = BlazorInteractive.Compilation.Results.CSharpCompilationResult;

namespace BlazorInteractive.Compilation;

public class CSharpCompiler : ICSharpCompiler
{
    public CSharpCompilationResult Compile(string sourceCode, string assemblyName, ReferenceCollection references, LanguageVersion languageVersion)
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

        var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(languageVersion);
        var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(sourceCodeWithUsings, parseOptions);

        var options = new CSharpCompilationOptions(
            OutputKind.DynamicallyLinkedLibrary,
            concurrentBuild: false,
            optimizationLevel: OptimizationLevel.Debug
        );

        ICSharpCompilationBuilder builder = new CSharpCompilationBuilder();
        var compilation = builder.Create(assemblyName, new[] { parsedSyntaxTree }, references, options);
        return new CSharpCompilationWrapper(compilation.Value);
    }
}