using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.ObjectModel;

using BlazorInteractive.Compilation;

public class CSharpCompiler : ICSharpCompiler
{
    public CSharpCompilationResult Compile(string sourceCode, string assemblyName, ReadOnlyCollection<MetadataReference> references)
    {
        if (string.IsNullOrEmpty(sourceCode))
        {
            return new Failure($"{nameof(sourceCode)} cannot be null or empty");
        }

        if (string.IsNullOrEmpty(assemblyName))
        {
            return new Failure($"{nameof(assemblyName)} cannot be null or empty");
        }

        var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Default);
        var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(sourceCode, options);
        
        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName,
            syntaxTrees: new[] { parsedSyntaxTree },
            references: references,
            options: new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                concurrentBuild: false,
                optimizationLevel: OptimizationLevel.Debug
            )
        );

        return compilation;
    }
}