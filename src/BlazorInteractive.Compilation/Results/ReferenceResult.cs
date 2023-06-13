using OneOf;

namespace BlazorInteractive.Compilation.Results;

[GenerateOneOf]
public partial class ReferenceResult : OneOfBase<ReferenceCollection, Failure, Cancelled> { }