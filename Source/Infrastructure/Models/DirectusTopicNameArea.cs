using Newtonsoft.Json;

namespace Infrastructure.Models;

public class DirectusTopicNameArea
{
    [JsonProperty("Sprache")] public DirectusTopicNameMultiLanguageBody[] MultiLanguageBody { get; set; }
}