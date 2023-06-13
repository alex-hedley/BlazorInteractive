namespace BlazorInteractive.Compilation.Extensions;

internal static class IEnumerableExtensions
{
    public static string Join(this IEnumerable<string> elems, string separator)
    {
        return string.Join(separator, elems);
    }
}