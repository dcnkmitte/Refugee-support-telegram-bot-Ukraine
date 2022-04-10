namespace Infrastructure.Telegram.Extensions;

public static class EnumerableExtensions
{
  public static bool IsEmpty<TSource>(this IEnumerable<TSource> source) => !source.Any();
}