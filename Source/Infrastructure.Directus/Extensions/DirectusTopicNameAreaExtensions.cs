using Infrastructure.Directus.Models;

namespace Infrastructure.Directus.Extensions;

public static class DirectusTopicNameAreaExtensions
{
    public static string? GetTopicNameIdeallyInPreferredLanguage(this DirectusTopicNameArea topicNameArea,
      string preferredLanguage)
    {
        var isPreferredLanguageTopicNamePresent =
          topicNameArea.MultiLanguageBody.Any(x => x.Language.Name.Equals(preferredLanguage) &&
          IsValidName(x));

        var topicName = isPreferredLanguageTopicNamePresent
          ? topicNameArea.MultiLanguageBody.First(x => x.Language.Name.Equals(preferredLanguage))
            .TopicName
          : topicNameArea.MultiLanguageBody.FirstOrDefault(x => IsValidName(x))?.TopicName;

        return topicName;
    }

    private static bool IsValidName(DirectusTopicNameMultiLanguageBody x) =>
        string.IsNullOrEmpty(x.TopicName) == false;
}