using Newtonsoft.Json;

namespace Infrastructure.Models;

public class DirectusTopicContentArea:IMultilanguage
{
    [JsonProperty("Inhalt")] public string TopicContent { get; set; }
    [JsonProperty("languages_id")] public DirectusLanguage Language { get; set; }
}