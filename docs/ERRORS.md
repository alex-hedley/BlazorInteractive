# Errors

## Compile from stream

**Code executed**

```csharp
Stream? requestBody = await GetRequestBodyAsStreamAsync(requestMessage, cancellationToken);
IJSStreamReference? result = await _accessorJsRef.Value.InvokeAsync<IJSStreamReference>("get", requestMessage.RequestUri, requestMethod, requestBody);
await using Stream stream = await result.OpenReadStreamAsync();
MemoryStream ms = new MemoryStream();
await stream.CopyToAsync(ms);
```

**Error message**

> 'Supplied value is not a typed array or blob.\nError: Supplied value is not a typed array or blob.\n    at p (http://localhost:5049/_framework/blazor.webassembly.js:1:1083)\n    at R (http://localhost:5049/_framework/blazor.webassembly.js:1:5037)\n    at http://localhost:5049/_framework/blazor.webassembly.js:1:3380'

**Note**
> Don't pass a `cancellationToken` to `IJSRuntime.Value.InvokeAsync`, as it will fail with confusing error around `Int.Ptr`

**Error message**

> 'Serialization and deserialization of 'System.IntPtr' instances are not supported. Path: $.WaitHandle.Handle.'

- https://github.com/dotnet/aspnetcore/issues/40799#issuecomment-1073999976

## Bad IL format

```csharp
foreach(var assemblyName in bootstrap.Assemblies())
{
    var message = new HttpRequestMessage(HttpMethod.Get, $"_framework/{assemblyName}");
    var assemblyAsString = await _storageAccessor.GetAsStringAsync(message);

    using var stream = new MemoryStream();
    using var writer = new StreamWriter(stream);
    {
        await writer.WriteAsync(assemblyAsString);
        await writer.FlushAsync();
        Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(stream);
        assemblies.Add(assembly);
    }
}
```

## Base64 to ByteArray

**Error message**

> Bad IL Format.
