using Newtonsoft.Json;

namespace Infrastructure.Directus.Models;

public enum DirectusItemStatus
{
  [JsonProperty("published")]
  Published = 1,

  [JsonProperty("draft")]
  Draft = 2,

  [JsonProperty("archived")]
  Archived = 3
}