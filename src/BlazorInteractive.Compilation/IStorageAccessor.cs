using System;

namespace BlazorInteractive.Compilation;

public interface IStorageAccessor : IAsyncDisposable
{
	Task<string> GetAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default);
}