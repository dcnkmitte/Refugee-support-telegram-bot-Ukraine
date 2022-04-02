using Infrastructure.Directus.Models;

namespace Infrastructure.Directus.Extensions;

public static class DirectusTopicContentAreaExtensions
{
  public static string? GetTopicContentInPreferredOrAnyLanguage(
    this IEnumerable<DirectusTopicContentArea> topicContentAreas,
    string preferredLanguage)
  {
    var topicContentsArray = topicContentAreas.ToArray();
    if (topicContentsArray.Length == 0) return null;

    var isTopicContentPresentInPreferredLanguage =
      topicContentsArray.Any(x => x.Language.Name.Equals(preferredLanguage));

    var topicContent = isTopicContentPresentInPreferredLanguage
      ? topicContentsArray.First(x => x.Language.Name.Equals(preferredLanguage)).TopicContent
      : topicContentsArray.FirstOrDefault()?.TopicContent;


    return topicContent;
  }
}