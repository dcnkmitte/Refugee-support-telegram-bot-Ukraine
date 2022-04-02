using Newtonsoft.Json;

namespace Infrastructure.Directus.Models;

public class DirectusTopicNameArea
{
  [JsonProperty("Sprache")] public DirectusTopicNameMultiLanguageBody[] MultiLanguageBody { get; set; }
}