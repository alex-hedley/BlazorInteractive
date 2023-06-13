using System.Collections.ObjectModel;

namespace BlazorInteractive.Compilation;

public class ReferenceCollection : ReadOnlyCollection<IReference>
{
    public ReferenceCollection(IList<IReference> references) : base(references)
    {
    }

    public ReferenceCollection(IEnumerable<IReference> references) : this(references.ToList())
    {
    }

    public IEnumerable<string> ToUsings()
    {
        return this
            .GetDistinctNamespaces()
            .Select(u => $"using {u};");
    }

    private IEnumerable<string> GetDistinctNamespaces()
    {
        return this
            .SelectMany(r => r.DistinctNamespaces())
            .Distinct()
            .Where(s => s is not null && s.StartsWith("System"))
            .Cast<string>()
            .OrderBy(s => s)
            .ToList();
    }
}