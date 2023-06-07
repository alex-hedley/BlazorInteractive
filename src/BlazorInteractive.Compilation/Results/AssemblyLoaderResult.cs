using System.Reflection;
using OneOf;

namespace BlazorInteractive.Compilation.Results;

[GenerateOneOf]
public partial class AssemblyLoaderResult : OneOfBase<Assembly, Failure> { }