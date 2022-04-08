using Dawn;
using Flurl;
using Flurl.Http;
using Infrastructure.Directus.Configuration;
using Infrastructure.Directus.Models;
using Infrastructure.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Directus;

public class DirectusService : IDirectusService
{
    private readonly ILogger<DirectusService> _log;
    private readonly Url _getTopicsUrl;
    private readonly Url _getConfigurationUrl;
    private readonly Url _postQuestionUrl;
    private readonly string _city;

    public DirectusService(IOptions<DirectusConfiguration> config, ILogger<DirectusService> log)
    {
        Guard.Argument(config.Value.AccessToken, "Directus:AccessToken").NotEmpty();
        _city = config.Value.City;
        Guard.Argument(_city, "Directus:City").NotEmpty();

        _log = log;
        _getTopicsUrl = "https://cms.nk-mitte.de/items/Inhalt".SetQueryParams(new
        {
            access_token = config.Value.AccessToken,
            fields =
                "status,date_created,date_updated,Stadt.Name,Sprache.Inhalt,Sprache.languages_id.*,Bereich.Sprache.Inhalt,Bereich.Sprache.languages_id.*,Bereich.Sprache.Bereich",
        })
          .SetQueryParam("deep[Stadt][_filter][Name][_eq]", _city)
          .SetQueryParam("filter[status]", DirectusItemStatus.Published);

        _getConfigurationUrl = "https://cms.nk-mitte.de/items/botconifguration".SetQueryParams(new
        {
            access_token = config.Value.AccessToken,
            fields =
                "aktualisierungsdatumanzeigen,translations.feedbacknachricht,translations.sondermeldung,translations.referenznachricht,translations.begruessungsnachricht,translations.languages_id.*,Sprache.*",
        })
      .SetQueryParam("deep[Stadt][_filter][Name][_eq]", _city);

        _postQuestionUrl = "https://cms.nk-mitte.de/items/fragen".SetQueryParams(new
        {
            access_token = config.Value.AccessToken,

        });


    }

    public async Task<DirectusTopic[]> GetTopicsAsync()
    {
        var topics = await _getTopicsUrl.GetJsonAsync<DirectusTopicWrapper>();

        return topics.Data;
    }

    public async Task<BotConfiguration[]> GetConfigurationAsync()
    {
        var topics = await _getConfigurationUrl.GetJsonAsync<BotConfigurationWrapper>();

        return topics.Data;
    }

    public async Task PostQuestionAsync(long chatId, string? question)
    {
        await _postQuestionUrl.PostJsonAsync(new
        {
            chatid = chatId,
            Stadt = _city,
            frage = question
        });
    }
}