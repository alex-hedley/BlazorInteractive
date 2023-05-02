using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

string source = "Console.WriteLine(\"Hello, World!\");";

string randomAssemblyName = Path.GetRandomFileName();

SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(
    source,
    CSharpParseOptions.Default.WithKind(SourceCodeKind.Script).WithLanguageVersion(LanguageVersion.Default)
);

IEnumerable<SyntaxTree> syntaxTrees = new List<SyntaxTree>() { syntaxTree };

string[] defaultNamespaces = new[] {
    "System",
    "System.IO",
    "System.Collections.Generic",
    "System.Console",
    "System.Diagnostics",
    "System.Dynamic",
    "System.Linq",
    "System.Linq.Expressions",
    "System.Net.Http",
    "System.Text",
    "System.Threading.Tasks"
};

CSharpCompilationOptions? options = new(
    outputKind: OutputKind.DynamicallyLinkedLibrary,
    usings: defaultNamespaces
);

Assembly[] appDomainAssemblies = AppDomain.CurrentDomain.GetAssemblies();

IList<MetadataReference> metadataReferences = new List<MetadataReference>();

foreach (Assembly? reference in appDomainAssemblies.Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location)))
{
    PortableExecutableReference metadataReference = MetadataReference.CreateFromFile(reference.Location);
    metadataReferences.Add(metadataReference);
}

CSharpCompilation compilation = CSharpCompilation.Create(
    randomAssemblyName,
    syntaxTrees,
    metadataReferences,
    options);

using MemoryStream peStream = new();

var emitResult = compilation.Emit(peStream);

// if (emitResult.Success)
// {
//     _submissionIndex++;
//     _previousCompilation = scriptCompilation;
//     assembly = Assembly.Load(peStream.ToArray());
// }