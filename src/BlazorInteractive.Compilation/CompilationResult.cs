using System.Reflection;
using OneOf;

namespace BlazorInteractive.Compilation;

[GenerateOneOf]
public partial class CompilationResult : OneOfBase<Assembly, Void, Failure, Cancelled> {}