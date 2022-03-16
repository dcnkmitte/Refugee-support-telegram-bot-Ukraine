using Newtonsoft.Json;

namespace Infrastructure.Directus.Models;

public class DirectusTopic
{
  [JsonProperty("date_created")] public DateTime DateCreated { get; set; } //2022-03-12T16:02:41

  [JsonProperty("date_updated")] public DateTime? DateUpdated { get; set; }

  [JsonProperty("Bereich")] public DirectusTopicTopicNameArea DetailsTopicNameArea { get; set; }

  [JsonProperty("Sprache")] public DirectusTopicContentArea[] DetailsTopicContentArea { get; set; }
}