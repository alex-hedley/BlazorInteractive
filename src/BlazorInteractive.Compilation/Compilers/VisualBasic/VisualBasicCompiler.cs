using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;

using BlazorInteractive.Compilation.Extensions;
using BlazorInteractive.Compilation.Results;

namespace BlazorInteractive.Compilation;

public class VisualBasicCompiler : IVisualBasicCompiler
{
    public VisualBasicCompilationResult Compile(string sourceCode, string assemblyName, ReferenceCollection references, long languageVersion)
    {
        if (string.IsNullOrEmpty(sourceCode))
        {
            return new Failure($"{nameof(sourceCode)} cannot be null or empty");
        }

        if (string.IsNullOrEmpty(assemblyName))
        {
            return new Failure($"{nameof(assemblyName)} cannot be null or empty");
        }

        var sourceCodeWithImports = references
            .ToImports()
            .Append(sourceCode)
            .Join(Environment.NewLine);

        var languageVersionEnum = (LanguageVersion)Enum.ToObject(typeof(LanguageVersion), languageVersion);
        var parseOptions = VisualBasicParseOptions.Default.WithLanguageVersion(languageVersionEnum);
        var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(sourceCodeWithImports, parseOptions);

        // BC35000: Requested operation is not available because the runtime library function '<function>' is not defined.
        // https://learn.microsoft.com/en-us/dotnet/visual-basic/language-reference/error-messages/bc35000
        // error BC35000: Requested operation is not available because the runtime library function 'Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute..ctor' is not defined.
        
        // *.vbproj 
        // <PropertyGroup>
        // <VBRuntime>Default</VBRuntime>
        
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