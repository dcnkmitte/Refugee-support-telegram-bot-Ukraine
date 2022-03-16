using Newtonsoft.Json;

namespace Infrastructure.Directus.Models;

public class DirectusTopicBody
{
  [JsonProperty("data")] public DirectusTopicBodyData[] Data { get; set; }
}

public class DirectusTopicBodyData
{
  [JsonProperty("content")] public string Content { get; set; }

  [JsonProperty("status")] public DirectusItemStatus Status { get; set; }

  [JsonProperty("id")] public int Id { get; set; }

  [JsonProperty("area")] public int Area { get; set; }

  [JsonProperty("date_created")] public DateTime DateCreated { get; set; } //2022-03-12T16:02:41

  [JsonProperty("date_updated")] public DateTime? DateUpdated { get; set; }
}