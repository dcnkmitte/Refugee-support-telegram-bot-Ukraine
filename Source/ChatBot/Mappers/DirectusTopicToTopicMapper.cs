using Infrastructure.Directus.Extensions;
using Infrastructure.Directus.Models;
using Infrastructure.Telegram.Models;

namespace ChatBot.Mappers;

public class DirectusTopicToTopicMapper : IMapper<DirectusTopic, Topic>
{
  private readonly string preferredLanguage;

  public DirectusTopicToTopicMapper(string preferredLanguage)
  {
    this.preferredLanguage = preferredLanguage;
  }

  public ICollection<Topic> Map(IEnumerable<DirectusTopic> directusTopics)
  {
    var result = new List<Topic>();

    foreach (var directusTopic in directusTopics)
    {
      var topicName = directusTopic.TopicNameArea.GetTopicNameInPreferredOrAnyLanguage(this.preferredLanguage);
      if (topicName == null) continue;

      var topicContent = directusTopic.TopicContentArea.GetTopicContentInPreferredOrAnyLanguage(this.preferredLanguage);
      if (topicContent == null) continue;

      var updatedDateTimeUtc = directusTopic.GetLastModifiedUtc();

      result.Add(new Topic(topicName, topicContent, updatedDateTimeUtc));
    }

    return result;
  }
}