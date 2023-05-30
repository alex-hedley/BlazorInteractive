using System.Reflection;
using OneOf;
using OneOf.Types;

namespace BlazorInteractive.Compilation;

[GenerateOneOf]
public partial class CompilationResult : OneOfBase<Assembly, Void, Failure, Cancelled> {}