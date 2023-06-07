using OneOf;

namespace BlazorInteractive.Compilation.Results;

[GenerateOneOf]
public partial class CSharpCompilationResult : OneOfBase<CSharpCompilationWrapper, Failure> {}