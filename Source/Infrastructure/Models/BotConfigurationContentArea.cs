using Newtonsoft.Json;

namespace Infrastructure.Models;

public class BotConfigurationContentArea : IMultilanguage
{
    [JsonProperty("feedbacknachricht")] public string Feedback { get; set; }
    [JsonProperty("sondermeldung")] public string Special { get; set; }
    [JsonProperty("referenznachricht")] public string Reference { get; set; }
    [JsonProperty("begruessungsnachricht")] public string Welcome { get; set; }
    [JsonProperty("languages_id")] public DirectusLanguage Language { get; set; }

}