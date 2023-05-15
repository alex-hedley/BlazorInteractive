
using Microsoft.CodeAnalysis;
using System.Collections.ObjectModel;
using System.Net;
using System.Reflection;

using Moq.Protected;

using BlazorInteractive.Compilation;

namespace BlazorInteractive.Compilation.Tests;

public class RemoteFileReferenceResolverTest
{
    private HttpClient _mockHttpClient;
    
    private RemoteFileReferenceResolver _remoteFileReferenceResolver;
    
    private readonly IEnumerable<Assembly> _defaultAssemblies;
    
    private readonly List<string> _defaultAssemblyNames;

    public RemoteFileReferenceResolverTest()
    {
        // _mockHttpClient = new Mock<HttpClient>();
        // _remoteFileReferenceResolver = new RemoteFileReferenceResolver(_mockHttpClient.Object);
        _defaultAssemblyNames = new List<string> { "System", "System.Collections" };
        _defaultAssemblies = _defaultAssemblyNames.Select(ass => Assembly.Load(ass)); //AppDomain.CurrentDomain.GetAssemblies();
    }

    [Fact]
    public async Task ResolveAsync_WithDefaultAssemblies_ReturnsItems()
    {
        var systemAssembly = _defaultAssemblies.First();
        using var systemStream = File.OpenRead(systemAssembly.Location);

        var mockMessageHandler = new Mock<HttpMessageHandler>();
        mockMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
            "SendAsync", 
            ItExpr.IsAny<HttpRequestMessage>(), 
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage request, CancellationToken _) =>
            {
                HttpResponseMessage response = new HttpResponseMessage();
                response.StatusCode = HttpStatusCode.OK;
                Console.WriteLine("==============");
                response.Content = new StreamContent(systemStream);
                return response;
            });

        _mockHttpClient = new HttpClient(mockMessageHandler.Object);
        _remoteFileReferenceResolver = new RemoteFileReferenceResolver(_mockHttpClient);

        //_mockHttpClient.Protected().Setup(m => m.GetStreamAsync(It.IsAny<string>())).ReturnsAsync(systemStream);
        // _mockHttpClient.Protected().Setup<Task<Stream>>("GetStreamAsync", ItExpr.IsAny<string>()).ReturnsAsync(systemStream);
        // _mockHttpClient.Protected().Setup<Task<Stream>>("GetStreamAsync", It.IsAny<string>()).ReturnsAsync(systemStream);

        // ASP.NET Core Unit Test: How to Mock HttpClient.GetStringAsync()
        // https://edi.wang/post/2021/5/11/aspnet-core-unit-test-how-to-mock-httpclientgetstringasync
        // var handlerMock = new Mock<HttpMessageHandler>();
        // var magicHttpClient = new HttpClient(handlerMock.Object);
        // handlerMock
        //     .Protected()
        //     .Setup<Task<HttpResponseMessage>>(
        //         "SendAsync",
        //         ItExpr.IsAny<HttpRequestMessage>(),
        //         ItExpr.IsAny<CancellationToken>()
        //     )
        //     .ReturnsAsync(new HttpResponseMessage
        //     {
        //         StatusCode = HttpStatusCode.OK,
        //         Content = new StringContent("the string you want to return")
        //     })
        //     .Verifiable();

        // // https://medium.com/younited-tech-blog/easy-httpclient-mocking-3395d0e5c4fa
        // // Instantiate the mock object
        // var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        // // Set up the SendAsync method behavior.
        // httpMessageHandlerMock
        //     .Protected() // <= here is the trick to set up protected!!!
        //     .Setup<Task<HttpResponseMessage>>(
        //         "SendAsync",
        //         ItExpr.IsAny<HttpRequestMessage>(),
        //         ItExpr.IsAny<CancellationToken>())
        //     .ReturnsAsync(new HttpResponseMessage());
        // // create the HttpClient
        // var httpClient = new HttpClient(httpMessageHandlerMock.Object);

        var result = await _remoteFileReferenceResolver.ResolveAsync(_defaultAssemblies);
        result.Value.Should().NotBeNull();
        result.Value.As<ReadOnlyCollection<MetadataReference>>().Should().NotBeEmpty();
    }

    // [Fact]
    // public async Task ResolveAsync_WithBadAssembly_ReturnsFailure()
    // {
    //     var badAssembly = new Mock<Assembly>();
    //     badAssembly.Setup(m => m.IsDynamic).Returns(false);
    //     badAssembly.Setup(m => m.Location).Returns("System.Cyberto");

    //     var baseAssemblies = new List<Assembly> { badAssembly.Object };
        
    //     var result = await _remoteFileReferenceResolver.ResolveAsync(baseAssemblies);

    //     result.Value.Should().BeOfType<Failure>();
    //     result.Value.As<Failure>().Exception.Should().BeOfType<FileNotFoundException>();
    // }

    // [Fact]
    // public async Task ResolveAsync_WithCancellationToken_ReturnsCancelled()
    // {
    //     var cancellationTokenSource = new CancellationTokenSource();
    //     var cancellationToken = cancellationTokenSource.Token;
        
    //     cancellationTokenSource.Cancel();
    //     var result = await _remoteFileReferenceResolver.ResolveAsync(_defaultAssemblies, cancellationToken);
    //     result.Value.Should().BeOfType<Cancelled>();
    // }
}