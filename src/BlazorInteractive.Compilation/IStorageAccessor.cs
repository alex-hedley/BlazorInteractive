using System;

namespace BlazorInteractive.Compilation;

public interface IStorageAccessor : IAsyncDisposable
{
	Task<string> GetAsStringAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default);

	Task<Stream> GetAsStreamAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default);
}