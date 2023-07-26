public class SampleArgs
{
    public string? Name { get; set; }

    public List<string>? AdditionalNamespaces { get; set; }

    public string? SourceCode { get; set; }
}

public class SourceCode
{
    public string? Language { get; set; }
    public string? Code { get; set; }
}

public class Sample
{
    public string? Name { get; set; }

    public List<string>? AdditionalNamespaces { get; set; }
    
    public List<SourceCode>? SourceCodes { get; set; }
}