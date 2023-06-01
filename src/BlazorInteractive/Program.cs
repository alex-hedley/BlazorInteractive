using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Collections.Immutable;

using BlazorInteractive;
using BlazorInteractive.AssemblyCompilation;
using BlazorInteractive.Compilation;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<ICompiler, CodeCompiler>();
builder.Services.AddScoped<IReferenceResolver, RemoteFileReferenceResolver>();
builder.Services.AddScoped<IStorageAccessor, CacheStorageAccessor>();
builder.Services.AddScoped<IAssemblyInvoker, AssemblyInvoker>();
builder.Services.AddScoped<IAssemblyLoader, AssemblyLoader>();
builder.Services.AddScoped<ICSharpCompiler, CSharpCompiler>();

builder.Services.AddHttpClient<IAssemblyAccessor<ImmutableArray<byte>>, BlazorAssemblyAccessor>(client => {
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

await builder.Build().RunAsync();