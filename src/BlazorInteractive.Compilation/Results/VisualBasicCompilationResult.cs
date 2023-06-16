using OneOf;

namespace BlazorInteractive.Compilation.Results;

[GenerateOneOf]
public partial class VisualBasicCompilationResult : OneOfBase<VisualBasicCompilationWrapper, Failure> {}