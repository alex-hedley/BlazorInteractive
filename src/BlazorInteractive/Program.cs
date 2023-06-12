using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Collections.Immutable;
using Blazored.Toast;

using BlazorInteractive;
using BlazorInteractive.AssemblyCompilation;
using BlazorInteractive.Compilation;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<ICompiler, CodeCompiler>();
builder.Services.AddScoped<IReferenceResolver, RemoteFileReferenceResolver>();
builder.Services.AddScoped<IAssemblyInvoker, AssemblyInvoker>();
builder.Services.AddScoped<IAssemblyLoader, AssemblyLoader>();
builder.Services.AddScoped<ICSharpCompiler, CSharpCompiler>();
builder.Services.AddBlazoredToast();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddHttpClient<IAssemblyAccessor<ImmutableArray<byte>>, BlazorAssemblyAccessor>(client => {
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

builder.Services.AddHttpClient<IReferenceFilter, BlazorReferenceFilter>(client => {
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

await builder.Build().RunAsync();