using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;
using OneOf;
using OneOf.Types;

namespace BlazorInteractive.Compilation;

[GenerateOneOf]
public partial class CSharpCompilationResult : OneOfBase<CSharpCompilation, Failure> {}