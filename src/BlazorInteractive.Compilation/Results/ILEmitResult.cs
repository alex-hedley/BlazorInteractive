using OneOf;

namespace BlazorInteractive.Compilation.Results;

[GenerateOneOf]
public partial class ILEmitResult : OneOfBase<string, Failure, Cancelled> { }
