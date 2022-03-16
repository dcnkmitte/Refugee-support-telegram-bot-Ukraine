using Newtonsoft.Json;

namespace Infrastructure.Directus.Models;

public class DirectusTopicContentArea
{
  [JsonProperty("Inhalt")] public string TopicContent { get; set; }
  [JsonProperty("languages_id")] public DirectusLanguage Language { get; set; }
}