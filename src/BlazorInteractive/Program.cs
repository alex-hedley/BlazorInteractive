using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Collections.Immutable;

using BlazorInteractive;
using BlazorInteractive.AssemblyCompilation;
using BlazorInteractive.Compilation;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<ScriptCompiler>();
builder.Services.AddScoped<IReferenceResolver, RemoteFileReferenceResolver>();
builder.Services.AddScoped<IStorageAccessor, CacheStorageAccessor>();

builder.Services.AddHttpClient<IAssemblyAccessor<ImmutableArray<byte>>, BlazorAssemblyAccessor>(client => {
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

await builder.Build().RunAsync();