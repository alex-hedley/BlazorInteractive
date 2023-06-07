using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.ObjectModel;

using BlazorInteractive.Compilation;

public class CSharpCompiler : ICSharpCompiler
{
    public CSharpCompilationResult Compile(string sourceCode, string assemblyName, ReadOnlyCollection<IReference> references)
    {
        if (string.IsNullOrEmpty(sourceCode))
        {
            return new Failure($"{nameof(sourceCode)} cannot be null or empty");
        }

        if (string.IsNullOrEmpty(assemblyName))
        {
            return new Failure($"{nameof(assemblyName)} cannot be null or empty");
        }

        var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Default);
        var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(sourceCode, parseOptions);
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