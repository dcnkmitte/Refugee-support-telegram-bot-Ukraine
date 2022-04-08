using Infrastructure.Directus.Models;
using Newtonsoft.Json;

namespace Infrastructure.Models;

public class QuestionsWrapper
{
    [JsonProperty("data")] public DirectusQuestion[] Data { get; set; }
}