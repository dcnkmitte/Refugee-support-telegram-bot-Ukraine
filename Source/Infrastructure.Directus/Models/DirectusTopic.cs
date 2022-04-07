using Newtonsoft.Json;

namespace Infrastructure.Directus.Models;

public class DirectusTopic
{
    [JsonProperty("date_created")] public DateTime DateCreated { get; set; }

    [JsonProperty("date_updated")] public DateTime? DateUpdated { get; set; }

    [JsonProperty("Bereich")] public DirectusTopicNameArea TopicNameArea { get; set; }

    [JsonProperty("Sprache")] public IEnumerable<DirectusTopicContentArea> TopicContentArea { get; set; }
}