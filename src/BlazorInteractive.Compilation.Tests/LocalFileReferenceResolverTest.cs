
using Microsoft.CodeAnalysis;
using System.Collections.ObjectModel;
using System.Reflection;

using BlazorInteractive.Compilation;

namespace BlazorInteractive.Compilation.Tests;

public class LocalFileReferenceResolverTest
{
    private readonly LocalFileReferenceResolver _localFileReferenceResolver;
    
    private readonly IEnumerable<Assembly> _defaultAssemblies;
    
    private readonly List<string> _defaultAssemblyNames;

    private readonly string _sourceCode = string.Empty;

    public LocalFileReferenceResolverTest()
    {
        _localFileReferenceResolver = new LocalFileReferenceResolver();
        _defaultAssemblyNames = new List<string> { "System", "System.Collections" };
        _defaultAssemblies = _defaultAssemblyNames.Select(ass => Assembly.Load(ass)); //AppDomain.CurrentDomain.GetAssemblies();
        _sourceCode = "Console.WriteLine(\"Hello, World!\");";
    }

    [Fact]
    public async Task ResolveAsync_WithDefaultAssemblies_ReturnsItems()
    {
        var result = await _localFileReferenceResolver.ResolveAsync(_defaultAssemblies);
        result.Value.Should().NotBeNull();
        result.Value.As<ReadOnlyCollection<MetadataReference>>().Should().NotBeEmpty();
    }

    [Fact]
    public async Task ResolveAsync_WithBadAssembly_ReturnsFailure()
    {
        var badAssembly = new Mock<Assembly>();
        badAssembly.Setup(m => m.IsDynamic).Returns(false);
        badAssembly.Setup(m => m.Location).Returns("System.Cyberto");

        var baseAssemblies = new List<Assembly> { badAssembly.Object };
        
        var result = await _localFileReferenceResolver.ResolveAsync(baseAssemblies);

        result.Value.Should().BeOfType<Failure>();
        result.Value.As<Failure>().Exception.Should().BeOfType<FileNotFoundException>();
    }

    [Fact]
    public async Task ResolveAsync_WithCancellationToken_ReturnsCancelled()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        
        cancellationTokenSource.Cancel();
        var result = await _localFileReferenceResolver.ResolveAsync(_defaultAssemblies, cancellationToken);
        result.Value.Should().BeOfType<Cancelled>();
    }
}