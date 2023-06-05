using OneOf;

namespace BlazorInteractive.Compilation;

[GenerateOneOf]
public partial class CSharpCompilationResult : OneOfBase<CSharpCompilationWrapper, Failure> {}