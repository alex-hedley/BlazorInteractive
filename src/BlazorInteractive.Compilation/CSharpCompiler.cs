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

        var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Default);
        var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(sourceCode, options);

        ICSharpCompilationBuilder builder = new CSharpCompilationBuilder();
        var compilation = builder.Create(assemblyName, new[] { parsedSyntaxTree }, references);
        return new CSharpCompilationWrapper(compilation.Value);
    }
}