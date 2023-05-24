using System;

namespace BlazorInteractive.Compilation;

public interface IStorageAccessor : IAsyncDisposable
{
	Task<string> GetAsStringAsync(HttpRequestMessage requestMessage);

	Task<byte[]> GetAsBytesAsync(HttpRequestMessage requestMessage);
}