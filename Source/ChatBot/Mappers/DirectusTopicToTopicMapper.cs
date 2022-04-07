using Infrastructure.Directus.Extensions;
using Infrastructure.Directus.Models;
using Infrastructure.Telegram.Models;

namespace ChatBot.Mappers;

public class DirectusTopicToTopicMapper : IMapper<DirectusTopic, Topic>
{
    private readonly string _preferredLanguage;

    public DirectusTopicToTopicMapper(string preferredLanguage) => _preferredLanguage = preferredLanguage;

    public ICollection<Topic> Map(IEnumerable<DirectusTopic> directusTopics)
    {
        var result = new List<Topic>();

        foreach (var directusTopic in directusTopics)
        {
            var topicName = directusTopic.TopicNameArea.GetTopicNameIdeallyInPreferredLanguage(_preferredLanguage);
            if (topicName == null) continue;

            var topicContent = directusTopic.TopicContentArea.GetTopicContentIdeallyInPreferredLanguage(_preferredLanguage);
            if (topicContent == null) continue;

            var updatedDateTimeUtc = directusTopic.GetLastModifiedUtc();

            var topic = new Topic(topicName, topicContent, updatedDateTimeUtc);
            result.Add(topic);
        }

        return result;
    }
}