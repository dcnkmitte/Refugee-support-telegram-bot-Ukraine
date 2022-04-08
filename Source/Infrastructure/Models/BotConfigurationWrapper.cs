using Newtonsoft.Json;

namespace Infrastructure.Models;

public class BotConfigurationWrapper
{
    [JsonProperty("data")] public BotConfiguration[] Data { get; set; }
}