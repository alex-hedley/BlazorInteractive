using Microsoft.CodeAnalysis;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis.Scripting;

namespace BlazorInteractive.Compilation;

public class RemoteFileReferenceResolver : IReferenceResolver
{
    private readonly IAssemblyAccessor<byte[]> _assemblyAccessor;

    public RemoteFileReferenceResolver(IAssemblyAccessor<byte[]> assemblyAccessor)
    {
        _assemblyAccessor = assemblyAccessor;
    }

    public async Task<ReferenceResult> ResolveAsync(IEnumerable<string> importNames, CancellationToken cancellationToken = default)
    {
        ReferenceResult result;

        if (cancellationToken.IsCancellationRequested) {
            result = new Cancelled();
            return result;
        }

        var assemblyBytes = await _assemblyAccessor.GetAsync(importNames, cancellationToken);
        
        return assemblyBytes.Match<ReferenceResult>(
            assemblies => {
                return assemblies
                        //TODO: Localtion doesn't exist on files loaded from cachestorage in blazor
                    .Select(a =>
                        {
                            unsafe
                            {
                                // Until MacOS supports TryGetRawMetadata, this will not work
                                Assembly assembly = LoadFromBytes(a);
                                assembly.TryGetRawMetadata(out byte* blob, out int length);                    
                                var moduleMetadata = ModuleMetadata.CreateFromMetadata((IntPtr)blob, length);
                                var assemblyMetadata = AssemblyMetadata.Create(moduleMetadata);
                                var metadataReference = assemblyMetadata.GetReference();
                                return metadataReference;
                            }
                        })
                    .Cast<MetadataReference>()
                    .ToList()
                    .AsReadOnly();
                    
            }, 
            failure => failure,
            cancelled => cancelled
        );
    }
    
    private static Assembly LoadFromBytes(byte[] assemblySource)
    {
        if (AssemblyLoadContext.Default is not null)
        {
            return AssemblyLoadContext.Default.LoadFromStream(new MemoryStream(assemblySource));
        }
        return Assembly.Load(assemblySource);
    }
}
