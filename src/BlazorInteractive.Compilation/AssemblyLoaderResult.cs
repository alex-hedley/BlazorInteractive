using System.Reflection;

using OneOf;

namespace BlazorInteractive.Compilation;

[GenerateOneOf]
public partial class AssemblyLoaderResult : OneOfBase<Assembly, Failure> { }