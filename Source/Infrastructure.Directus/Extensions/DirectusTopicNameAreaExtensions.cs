using Infrastructure.Directus.Models;

namespace Infrastructure.Directus.Extensions;

public static class DirectusTopicNameAreaExtensions
{
  public static string? GetTopicNameInPreferredOrAnyLanguage(this DirectusTopicNameArea topicNameArea,
    string preferredLanguage)
  {
    var isTopicNamePresentInPreferredLanguage =
      topicNameArea.MultiLanguageBody.Any(x => x.Language.Name.Equals(preferredLanguage));

    var topicName = isTopicNamePresentInPreferredLanguage
      ? topicNameArea.MultiLanguageBody.First(x => x.Language.Name.Equals(preferredLanguage))
        .TopicName
      : topicNameArea.MultiLanguageBody.FirstOrDefault()?.TopicName;

    return topicName;
  }
}