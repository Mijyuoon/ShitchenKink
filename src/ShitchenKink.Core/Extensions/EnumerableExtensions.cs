namespace ShitchenKink.Core.Extensions;

public static class EnumerableExtensions
{
    public static string JoinString<T>(this IEnumerable<T> source, string? separator)
        => String.Join(separator, source);
}