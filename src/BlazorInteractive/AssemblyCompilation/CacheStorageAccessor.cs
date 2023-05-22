// Cache storage
// https://blazorschool.com/tutorial/blazor-wasm/dotnet6/cache-storage-658620

using Microsoft.JSInterop;

using BlazorInteractive.Compilation;

namespace BlazorInteractive.AssemblyCompilation;

public class CacheStorageAccessor : IStorageAccessor
{
    private Lazy<IJSObjectReference> _accessorJsRef = new();
    private readonly IJSRuntime _jsRuntime;

    public CacheStorageAccessor(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async ValueTask DisposeAsync()
    {
        if (_accessorJsRef.IsValueCreated)
        {
            await _accessorJsRef.Value.DisposeAsync();
        }
    }

    public async Task<string> GetAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
    {
        await WaitForReference(cancellationToken);
        string requestMethod = requestMessage.Method.Method;
        string requestBody = await GetRequestBodyAsync(requestMessage, cancellationToken);
        string result = await _accessorJsRef.Value.InvokeAsync<string>("get", requestMessage.RequestUri, requestMethod, requestBody);

        return result;
    }

    private async Task WaitForReference(CancellationToken cancellationToken)
    {
        if (_accessorJsRef.IsValueCreated is false)
        {
            _accessorJsRef = new(await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/cacheStorageAccessor.js", cancellationToken));
        }
    }

    private static async Task<string> GetRequestBodyAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
    {
        string requestBody = string.Empty;

        if (requestMessage.Content is not null)
        {
            requestBody = await requestMessage.Content.ReadAsStringAsync() ?? string.Empty;
        }

        return requestBody;
    }
}