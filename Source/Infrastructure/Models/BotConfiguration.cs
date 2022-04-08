using Newtonsoft.Json;

namespace Infrastructure.Models;

public class BotConfiguration
{
    [JsonProperty("aktualisierungsdatumanzeigen")] public bool ShowLastUpdadeDate { get; set; }
    [JsonProperty("translations")] public IEnumerable<BotConfigurationContentArea> ConfigurationContentArea { get; set; }
}