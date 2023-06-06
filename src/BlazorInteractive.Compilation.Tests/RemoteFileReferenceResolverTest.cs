using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Net;
using System.Reflection;

using Moq.Protected;

using static BlazorInteractive.Compilation.Tests.RemoteFileReferenceResolverTestData;

namespace BlazorInteractive.Compilation.Tests;

public class RemoteFileReferenceResolverTest
{
    private readonly Mock<ILogger<RemoteFileReferenceResolver>> _logger;
    private readonly Mock<IAssemblyAccessor<ImmutableArray<byte>>> _assemblyAccessor;
    private readonly CancellationToken _defaultCancellationToken;

    private Mock<IStorageAccessor> _storageAccessor;

    public RemoteFileReferenceResolverTest()
    {
        _assemblyAccessor = new Mock<IAssemblyAccessor<ImmutableArray<byte>>>();
        _defaultCancellationToken = CancellationToken.None;
        _logger = new Mock<ILogger<RemoteFileReferenceResolver>>();
        _storageAccessor = new Mock<IStorageAccessor>();
    }

    [Theory]
    [ClassData(typeof(RemoteFileReferenceResolverTestData))]
    public async Task ResolveAsync_WithDefaultAssemblies_ReturnsItems(string assemblyName)
    {
        Assembly assembly = Assembly.Load(assemblyName);
        var assemblyBytes = File.ReadAllBytes(assembly.Location);
        var assemblyImmutableBytes = ImmutableArray.Create(assemblyBytes);
        var readonlyList = new ReadOnlyCollection<ImmutableArray<byte>>(new List<ImmutableArray<byte>> { assemblyImmutableBytes } );

        _assemblyAccessor.Setup(a => a.GetAsync(new[] { assemblyName }, _defaultCancellationToken)).ReturnsAsync(readonlyList);

        var remoteFileReferenceResolver = new RemoteFileReferenceResolver(_assemblyAccessor.Object, _logger.Object);

        var result = await remoteFileReferenceResolver.ResolveAsync(new [] { assemblyName }, _defaultCancellationToken);
        result.Value.Should().NotBeNull();
        result.Value.As<ReadOnlyCollection<IReference>>().Should().NotBeEmpty();

    }

    [Fact]
    public async Task ResolveAsync_WithBadAssembly_ReturnsFailure()
    {
        _assemblyAccessor.Setup(a => a.GetAsync(new [] { InvalidAssembly }, _defaultCancellationToken)).ReturnsAsync(new Failure(string.Empty));
        var remoteFileReferenceResolver = new RemoteFileReferenceResolver(_assemblyAccessor.Object, _logger.Object);

        var result = await remoteFileReferenceResolver.ResolveAsync(new [] { InvalidAssembly }, _defaultCancellationToken);
        result.Value.Should().BeOfType<Failure>();
    }

    [Fact]
    public async Task ResolveAsync_WithCancellationToken_ReturnsCancelled()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var remoteFileReferenceResolver = new RemoteFileReferenceResolver(_assemblyAccessor.Object, _logger.Object);

        cancellationTokenSource.Cancel();
        var result = await remoteFileReferenceResolver.ResolveAsync(new[] { SystemAssembly }, cancellationToken);
        result.Value.Should().BeOfType<Cancelled>();
    }
}

public class RemoteFileReferenceResolverTestData : IEnumerable<object[]>
{
    public const string SystemAssembly = "System";
    public const string SystemCollectionsAssembly = "System.Collections";
    public const string InvalidAssembly = "System.Cyberto";

    public const string BaseAddress = "https://localhost";

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { SystemAssembly };
        yield return new object[] { SystemCollectionsAssembly };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}