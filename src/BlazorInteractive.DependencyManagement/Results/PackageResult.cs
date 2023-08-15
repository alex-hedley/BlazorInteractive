using OneOf;
using OneOf.Types;

namespace BlazorInteractive.DependencyManagement;

[GenerateOneOf]
public partial class PackageResult : OneOfBase<Success, Failure> { }
