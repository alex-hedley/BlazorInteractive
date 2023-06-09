using System.Collections.ObjectModel;
using OneOf;

namespace BlazorInteractive.Compilation.Results;

[GenerateOneOf]
public partial class ReferenceFilterResult : OneOfBase<ReadOnlyCollection<string>, Failure, Cancelled>
{
}