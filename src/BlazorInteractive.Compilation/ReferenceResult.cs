using System.Collections.ObjectModel;

using OneOf;

namespace BlazorInteractive.Compilation;

[GenerateOneOf]
public partial class ReferenceResult : OneOfBase<ReadOnlyCollection<IReference>, Failure, Cancelled> { }