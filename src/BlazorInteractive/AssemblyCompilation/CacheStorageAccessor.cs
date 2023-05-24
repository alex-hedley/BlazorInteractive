// Cache storage
// https://blazorschool.com/tutorial/blazor-wasm/dotnet6/cache-storage-658620

using System.Text;
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

    public async Task<string> GetAsStringAsync(HttpRequestMessage requestMessage)
    {
        await WaitForReference();
        string requestMethod = requestMessage.Method.Method;
        string requestBody = await GetRequestBodyAsStringAsync(requestMessage);
        string result = await _accessorJsRef.Value.InvokeAsync<string>("get", requestMessage.RequestUri, requestMethod, requestBody);

        return result;
    }

    public async Task<byte[]> GetAsBytesAsync(HttpRequestMessage requestMessage)
    {
        await WaitForReference();
        string requestMethod = requestMessage.Method.Method;
        string requestBody = await GetRequestBodyAsStringAsync(requestMessage);
        string base64Result = await _accessorJsRef.Value.InvokeAsync<string>("getBase64AsBytes", requestMessage.RequestUri, requestMethod, requestBody);
        byte[] result = Convert.FromBase64String(base64Result);
        return result;
    }

    private async Task WaitForReference()
    {
        if (_accessorJsRef.IsValueCreated is false)
        {
            _accessorJsRef = new(await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/cacheStorageAccessor.js"));
        }
    }

    private static async Task<string> GetRequestBodyAsStringAsync(HttpRequestMessage requestMessage)
    {
        string requestBody = string.Empty;

        if (requestMessage.Content is not null)
        {
            requestBody = await requestMessage.Content.ReadAsStringAsync() ?? string.Empty;
        }

        return requestBody;
    }
}