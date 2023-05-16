
using Microsoft.CodeAnalysis;
using System.Collections.ObjectModel;
using System.Net;
using System.Reflection;

using Moq.Protected;

using System.Collections;

using static BlazorInteractive.Compilation.Tests.RemoteFileReferenceResolverTestData;

namespace BlazorInteractive.Compilation.Tests;

public class RemoteFileReferenceResolverTest
{
    [Theory]
    [ClassData(typeof(RemoteFileReferenceResolverTestData))]
    public async Task ResolveAsync_WithDefaultAssemblies_ReturnsItems(string assemblyName)
    {
        Assembly assembly = Assembly.Load(assemblyName);
        using var fileStream = File.OpenRead(assembly.Location);

        var mockMessageHandler = new Mock<HttpMessageHandler>();
        mockMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage request, CancellationToken _) =>
            {
                HttpResponseMessage response = new()
                {
                    StatusCode = HttpStatusCode.OK
                };
                var length = fileStream.Length.ToString();
                var streamContent = new StreamContent(fileStream);
                streamContent.Headers.Add("Content-Type", "application/octet-stream");
                streamContent.Headers.Add("Content-Length", length);
                response.Content = streamContent;

                return response;
            });

        var httpClient = CreateHttpClient(BaseAddress, mockMessageHandler.Object);
        var remoteFileReferenceResolver = new RemoteFileReferenceResolver(httpClient);

        var result = await remoteFileReferenceResolver.ResolveAsync(new[] { assembly });
        result.Value.Should().NotBeNull();
        result.Value.As<ReadOnlyCollection<MetadataReference>>().Should().NotBeEmpty();
    }

    [Fact]
    public async Task ResolveAsync_WithBadAssembly_ReturnsFailure()
    {
        var badAssembly = new Mock<Assembly>();
        badAssembly.Setup(m => m.IsDynamic).Returns(false);
        badAssembly.Setup(m => m.Location).Returns(InvalidAssembly);

        var baseAssemblies = new List<Assembly> { badAssembly.Object };
        var remoteFileReferenceResolver = new RemoteFileReferenceResolver(CreateHttpClient(BaseAddress));

        var result = await remoteFileReferenceResolver.ResolveAsync(baseAssemblies);

        result.Value.Should().BeOfType<Failure>();
        result.Value.As<Failure>().Exception.Should().BeOfType<HttpRequestException>();
    }

    [Fact]
    public async Task ResolveAsync_WithCancellationToken_ReturnsCancelled()
    {
        Assembly assembly = Assembly.Load(SystemAssembly);
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var remoteFileReferenceResolver = new RemoteFileReferenceResolver(CreateHttpClient(BaseAddress));

        cancellationTokenSource.Cancel();
        var result = await remoteFileReferenceResolver.ResolveAsync(new[] { assembly }, cancellationToken);
        result.Value.Should().BeOfType<Cancelled>();
    }

    private HttpClient CreateHttpClient(string baseAddress, HttpMessageHandler? handler = default)
    {
        var client = handler is null ? new HttpClient() : new HttpClient(handler);
        client.BaseAddress = new Uri(baseAddress);
        return client;
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