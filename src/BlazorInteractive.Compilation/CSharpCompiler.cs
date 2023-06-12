using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.ObjectModel;

using BlazorInteractive.Compilation;
using BlazorInteractive.Compilation.Results;
using CSharpCompilationResult = BlazorInteractive.Compilation.Results.CSharpCompilationResult;

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

        // remove: debug
        IEnumerable<string> usings = references.SelectMany(r => r.DistinctNamespaces()).Distinct().Where(s => s is not null).Where(s => s.StartsWith("System")).Cast<string>().OrderBy(s => s).ToList();
        var namespaceList = string.Join(Environment.NewLine, usings.Select(u => $"using {u};"));

        var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Default);
        var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(namespaceList + Environment.NewLine + sourceCode, parseOptions);

        var options = new CSharpCompilationOptions(
            OutputKind.DynamicallyLinkedLibrary,
            concurrentBuild: false,
            optimizationLevel: OptimizationLevel.Debug,
            usings: usings
        );

        ICSharpCompilationBuilder builder = new CSharpCompilationBuilder();
        var compilation = builder.Create(assemblyName, new[] { parsedSyntaxTree }, references, options);
        return new CSharpCompilationWrapper(compilation.Value);
    }
}