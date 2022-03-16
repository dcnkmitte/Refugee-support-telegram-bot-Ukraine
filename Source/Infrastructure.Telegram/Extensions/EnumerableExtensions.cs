namespace Infrastructure.Telegram.Extensions;

public static class EnumerableExtensions
{
  public static bool Empty<TSource>(this IEnumerable<TSource> source) => !source.Any();
}