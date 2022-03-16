using Newtonsoft.Json;

namespace Infrastructure.Directus.Models;

public class DirectusTopicName
{
  [JsonProperty("data")]
  public DirectusTopicNameData[] Data { get; set; }
}

public class DirectusTopicNameData
{
  [JsonProperty("name")] public string Name { get; set; }

  [JsonProperty("status")] public DirectusItemStatus Status { get; set; }

  [JsonProperty("id")] public int Id { get; set; }
  [JsonProperty("city")] public int CityId { get; set; }

  [JsonProperty("area")] public int Area { get; set; }
}