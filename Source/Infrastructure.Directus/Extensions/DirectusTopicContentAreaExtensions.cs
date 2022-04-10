using Infrastructure.Directus.Models;

namespace Infrastructure.Directus.Extensions;

public static class DirectusTopicContentAreaExtensions
{
    public static string? GetTopicContentIdeallyInPreferredLanguage(
      this IEnumerable<DirectusTopicContentArea> topicContentAreas,
      string preferredLanguage)
    {
        var topicContentsArray = topicContentAreas.ToArray();
        if (topicContentsArray.Length == 0) return null;

        var isPreferredLanguageTopicContentPresent =
          topicContentsArray.Any(x => x.Language.Name.Equals(preferredLanguage) &&
           HasValidContent(x));

        var topicContent = isPreferredLanguageTopicContentPresent
          ? topicContentsArray.First(x => x.Language.Name.Equals(preferredLanguage)).TopicContent
          : topicContentsArray.FirstOrDefault(x => HasValidContent(x))?.TopicContent;

        return topicContent;
    }

    private static bool HasValidContent(DirectusTopicContentArea x) =>
        string.IsNullOrEmpty(x.TopicContent) == false;
}