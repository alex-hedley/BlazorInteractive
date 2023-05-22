
using Microsoft.CodeAnalysis;
using System.Collections.ObjectModel;
using System.Reflection;

using BlazorInteractive.Compilation;

namespace BlazorInteractive.Compilation.Tests;

using static BlazorInteractive.Compilation.Tests.LocalFileReferenceResolverTestData;

public class LocalFileReferenceResolverTest
{
    private static IEnumerable<Assembly> _appDomainAssemblies;
    private readonly Mock<IAssemblyAccessor> _assemblyAccessor;
    private readonly CancellationToken _defaultCancellationToken;
    private readonly Mock<IStorageAccessor> _storageAccessor;

    private readonly LocalFileReferenceResolver _localFileReferenceResolver;
    
    private readonly List<string> _defaultAssemblyNames;

    static LocalFileReferenceResolverTest()
    {
        _appDomainAssemblies = AppDomain.CurrentDomain.GetAssemblies();
    }

    public LocalFileReferenceResolverTest()
    {
        _assemblyAccessor = new Mock<IAssemblyAccessor>();
        _defaultCancellationToken = CancellationToken.None;
        _storageAccessor = new Mock<IStorageAccessor>();
        _localFileReferenceResolver = new LocalFileReferenceResolver(_assemblyAccessor.Object);
        _defaultAssemblyNames = new List<string> { SystemAssembly, SystemCollectionsAssembly };
    }

    [Fact]
    public async Task ResolveAsync_WithDefaultAssemblies_ReturnsItems()
    {
        _assemblyAccessor.Setup(a => a.GetAsync(_defaultCancellationToken)).ReturnsAsync(_appDomainAssemblies.ToList().AsReadOnly());

        var result = await _localFileReferenceResolver.ResolveAsync(_defaultAssemblyNames);
        result.Value.Should().NotBeNull();
        result.Value.As<ReadOnlyCollection<MetadataReference>>().Should().NotBeEmpty();
    }

    [Fact]
    public async Task ResolveAsync_WithCreateFromFile_FileNotFound_ReturnsFailure()
    {        
        var badAssembly = new Mock<Assembly>();
        badAssembly.Setup(m => m.FullName).Returns(InvalidAssembly);
        badAssembly.Setup(m => m.IsDynamic).Returns(false);
        badAssembly.Setup(m => m.Location).Returns(InvalidAssembly);
        
        var assemblies = new [] { badAssembly.Object }.ToList().AsReadOnly();
        _assemblyAccessor.Setup(a => a.GetAsync(_defaultCancellationToken)).ReturnsAsync(assemblies);
        
        var baseAssemblies = new List<Assembly> { badAssembly.Object };
        var baseAssemblyNames = baseAssemblies.Select(a => a.FullName).Where(s => s is not null);
        
        var result = await _localFileReferenceResolver.ResolveAsync(baseAssemblyNames!);
        result.Value.Should().BeOfType<Failure>();
        result.Value.As<Failure>().Exception.Should().BeOfType<FileNotFoundException>();
    }

    [Fact]
    public async Task ResolveAsync_WithGetAssemblyFailure_ReturnsFailure()
    {
        AppDomainAssemblyException exception = new AppDomainAssemblyException();
        _assemblyAccessor.Setup(a => a.GetAsync(_defaultCancellationToken)).ReturnsAsync(new Failure(exception, exception.Message));
        
        var result = await _localFileReferenceResolver.ResolveAsync(_defaultAssemblyNames);
        result.Value.Should().BeOfType<Failure>();
        result.Value.As<Failure>().Exception.Should().BeOfType<AppDomainAssemblyException>();
    }

    [Fact]
    public async Task ResolveAsync_WithCancellationToken_ReturnsCancelled()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        
        cancellationTokenSource.Cancel();
        var result = await _localFileReferenceResolver.ResolveAsync(_defaultAssemblyNames, cancellationToken);
        result.Value.Should().BeOfType<Cancelled>();
    }

    [Fact]
    public async Task ResolveAsync_WithGetAssemblyCancelled_ReturnsCancelled()
    {
        _assemblyAccessor.Setup(a => a.GetAsync(_defaultCancellationToken)).ReturnsAsync(new Cancelled());

        var result = await _localFileReferenceResolver.ResolveAsync(_defaultAssemblyNames, _defaultCancellationToken);

        result.Value.Should().BeOfType<Cancelled>();
    }
}

public class LocalFileReferenceResolverTestData
{
    public const string SystemAssembly = "System";
    public const string SystemCollectionsAssembly = "System.Collections";
    public const string InvalidAssembly = "System.Cyberto";
}