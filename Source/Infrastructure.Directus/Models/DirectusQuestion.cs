using Infrastructure.Models;
using Newtonsoft.Json;

namespace Infrastructure.Directus.Models;

public class DirectusQuestion
{
    [JsonProperty("id")] public int id { get; set; }

    [JsonProperty("chatid")] public string ChatId { get; set; }

    [JsonProperty("frage")] public string Question { get; set; }

    [JsonProperty("antwort")] public string Answer { get; set; }
}