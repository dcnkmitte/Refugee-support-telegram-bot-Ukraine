using Infrastructure.Models;
using Newtonsoft.Json;

namespace Infrastructure.Directus.Models;

public class DirectusTopicNameMultiLanguageBody : IMultilanguage
{
    [JsonProperty("Bereich")] public string TopicName { get; set; }

    [JsonProperty("languages_id")] public DirectusLanguage Language { get; set; }
}