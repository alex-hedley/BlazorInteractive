using OneOf;

namespace BlazorInteractive.Compilation;

public sealed record Failure(string errorMessage) {

    public Failure(Exception ex, string errorMessage) : this(errorMessage)
    {
        Exception = ex;
    }

    public Exception? Exception { get; init; }
}