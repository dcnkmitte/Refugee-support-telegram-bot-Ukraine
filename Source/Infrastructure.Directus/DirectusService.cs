using Dawn;
using Flurl;
using Flurl.Http;
using Infrastructure.Directus.Configuration;
using Infrastructure.Directus.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Directus;

public class DirectusService : IDirectusService
{
  private readonly ILogger<DirectusService> _log;
  private readonly Url _getTopicsUrl;

  public DirectusService(IOptions<DirectusConfiguration> config, ILogger<DirectusService> log)
  {
    Guard.Argument(config.Value.AccessToken, "Directus:AccessToken").NotEmpty();
    Guard.Argument(config.Value.City, "Directus:City").NotEmpty();
    Guard.Argument(config.Value.PreferredLanguage, "Directus:PreferredLanguage").NotEmpty();

    PreferredLanguage = config.Value.PreferredLanguage;

    _log = log;
    _getTopicsUrl = "https://cms.nk-mitte.de/items/Inhalt".SetQueryParams(new
      {
        access_token = config.Value.AccessToken,
        fields =
          "status,date_created,date_updated,Stadt.Name,Sprache.Inhalt,Sprache.languages_id.Name,Bereich.Sprache.Inhalt,Bereich.Sprache.languages_id.Name,Bereich.Sprache.Bereich",
      })
      .SetQueryParam("deep[Stadt][_filter][Name][_eq]", config.Value.City)
      .SetQueryParam("filter[status]", DirectusItemStatus.Published);
  }

  public string PreferredLanguage { get; }

  public async Task<DirectusTopic[]> GetTopicsAsync()
  {
    var topics = await _getTopicsUrl.GetJsonAsync<DirectusTopicWrapper>();

    return topics.Data;
  }
}