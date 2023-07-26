// using Microsoft.CodeAnalysis;
//
// namespace BlazorInteractive.Compilation;
//
// public class CompilationBuilder : ICompilationBuilder
// {
//     public ICompilation Create(string? assemblyName, IEnumerable<SyntaxTree>? syntaxTrees = null, IEnumerable<IReference>? references = null, CompilationOptions? options = null)
//     {
//         return new CompilationWrapper(Compilation.Create(assemblyName, syntaxTrees, references?.Select(r => r.Value), options));
//     }
// }