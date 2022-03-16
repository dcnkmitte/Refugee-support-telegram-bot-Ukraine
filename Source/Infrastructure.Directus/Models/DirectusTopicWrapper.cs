using Newtonsoft.Json;

namespace Infrastructure.Directus.Models;

public class DirectusTopicWrapper
{
  [JsonProperty("data")] public DirectusTopic[] Data { get; set; }
}