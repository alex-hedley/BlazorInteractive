using System.Collections.ObjectModel;
using Microsoft.CodeAnalysis;
using OneOf;

namespace BlazorInteractive.Compilation;

[GenerateOneOf]
public partial class ReferenceResult : OneOfBase<ReadOnlyCollection<IReference>, Failure, Cancelled> { }