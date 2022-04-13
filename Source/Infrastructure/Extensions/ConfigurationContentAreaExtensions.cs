namespace Infrastructure.Extensions;

public static class ConfigurationContentAreaExtensions
{
    public static string? GetIdeallyInPreferredLanguage<T>(
      this IEnumerable<T> topicContentAreas,
      string preferredLanguage,
      Func<T, string> fieldToUse)
        where T : IMultilanguage
    {
        var topicContentsArray = topicContentAreas.ToArray();
        if (topicContentsArray.Length == 0) return null;

        var isPreferredLanguageTopicContentPresent =
          topicContentsArray.Any(x => x.Language.Name.Equals(preferredLanguage) &&
           HasValidContent(fieldToUse(x)));

        var content = isPreferredLanguageTopicContentPresent
          ? topicContentsArray.First(x => x.Language.Name.Equals(preferredLanguage))
          : topicContentsArray.FirstOrDefault(x => HasValidContent(fieldToUse(x)));

        var result = content != null ? fieldToUse(content) : null;
        return result;
    }

    private static bool HasValidContent(string x) => string.IsNullOrEmpty(x) == false;
}