// using Microsoft.CodeAnalysis;
// using Microsoft.CodeAnalysis.CSharp;
// // using Microsoft.CodeAnalysis.VisualBasic;
//
// using BlazorInteractive.Compilation.Extensions;
// using BlazorInteractive.Compilation.Results;
//
// namespace BlazorInteractive.Compilation;
//
// public class Compiler : ICompiler
// {
//     public CompilationResult Compile(string sourceCode, string assemblyName, ReferenceCollection references, LanguageVersion languageVersion)
//     {
//         if (string.IsNullOrEmpty(sourceCode))
//         {
//             return new Failure($"{nameof(sourceCode)} cannot be null or empty");
//         }
//
//         if (string.IsNullOrEmpty(assemblyName))
//         {
//             return new Failure($"{nameof(assemblyName)} cannot be null or empty");
//         }
//
//         var sourceCodeWithUsings = references
//             .ToUsings()
//             .Append(sourceCode)
//             .Join(Environment.NewLine);
//
//         // var parseOptions = ParseOptions.Default.WithLanguageVersion(languageVersion);
//         // ParseOptions.Default
//         // var parseOptions = new ParseOptions(SourceCodeKind.Script, DocumentationMode.None);
//         // var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(sourceCodeWithUsings, parseOptions);
//         //
//         // var options = new CompilationOptions(
//         //     OutputKind.DynamicallyLinkedLibrary,
//         //     concurrentBuild: false,
//         //     optimizationLevel: OptimizationLevel.Debug
//         // );
//         //
//         // ICompilationBuilder builder = new CompilationBuilder();
//         // var compilation = builder.Create(assemblyName, new[] { parsedSyntaxTree }, references, options);
//         // return new CompilationWrapper(compilation.Value);
//     }
// }