using Infrastructure.Extensions;
using Newtonsoft.Json;

namespace Infrastructure.Models;

public class BotConfiguration
{
    [JsonProperty("aktualisierungsdatumanzeigen")] public bool ShowLastUpdadeDate { get; set; }
    [JsonProperty("Sprache")] public DirectusLanguage PreferredLanguage { get; set; }
    [JsonProperty("translations")] public IEnumerable<BotConfigurationContentArea> ConfigurationContentArea { get; set; }
}