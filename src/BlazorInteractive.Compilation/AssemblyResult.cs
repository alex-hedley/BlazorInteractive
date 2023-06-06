using System.Collections.ObjectModel;

using OneOf;

namespace BlazorInteractive.Compilation;

[GenerateOneOf]
public partial class AssemblyResult<TAssembly> : OneOfBase<ReadOnlyCollection<TAssembly>, Failure, Cancelled> { }