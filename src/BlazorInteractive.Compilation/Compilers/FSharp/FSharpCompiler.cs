// // using Microsoft.FSharp.Core.CompilerServices;
// using FSharp.Compiler.CodeAnalysis;
// using FSharp.Compiler.CodeAnalysis.FSharpChecker;
//
// namespace BlazorInteractive.Compilation;
//
// public class FSharpCompiler
// {
//     public void Compile()
//     {
//         // Compiling and Executing F# Dynamically at Runtime
//         // https://www.jamesdrandall.com/posts/compiling_and_executing_fsharp_dynamically_at_runtime/
//         
//         // FSharpChecker.Create()
//         // checker.CompileToDynamicAssembly([|
//         //     "-o" ; outputAssemblyName
//         //     "-a" ; script
//         //     "--targetprofile:netcore"
//
//         var checker = FSharpChecker.Create();
//         checker.Compile();
//         
//         // https://github.com/stryker-mutator/stryker-net/blob/master/ExampleProjects/fsharpsyntaxtrees/fsharpsyntaxtrees/Program.cs
//         // https://github.com/stryker-mutator/stryker-net/blob/master/src/Stryker.Core/Stryker.Core/Compiling/FsharpCompilingProcess.cs
//     }
// }