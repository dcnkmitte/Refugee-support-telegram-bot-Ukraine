using Newtonsoft.Json;

namespace Infrastructure.Directus.Models;

public class DirectusTopicTopicNameArea
{
  [JsonProperty("Sprache")] public DirectusTopicNameMultiLanguageBody[] MultiLanguageBody { get; set; }
}