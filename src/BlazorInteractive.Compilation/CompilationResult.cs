using OneOf;

namespace BlazorInteractive.Compilation;

[GenerateOneOf]
public partial class CompilationResult : OneOfBase<Success, Void, Failure, Cancelled> {}