using System.Reflection;
using OneOf;

namespace BlazorInteractive.Compilation.Results;

[GenerateOneOf]
public partial class CompilationResult : OneOfBase<Assembly, Void, Failure, Cancelled> {}