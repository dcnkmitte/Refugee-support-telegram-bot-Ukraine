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
    private readonly Url _questionUrl;
    private readonly string _city;
    private readonly string _accessToken;

    public DirectusService(IOptions<DirectusConfiguration> config, ILogger<DirectusService> log)
    {
        _accessToken = config.Value.AccessToken;
        Guard.Argument(_accessToken, "Directus:AccessToken").NotEmpty();
        _city = config.Value.City;
        Guard.Argument(_city, "Directus:City").NotEmpty();

        _log = log;
        _getTopicsUrl = "https://cms.nk-mitte.de/items/Inhalt".SetQueryParams(new
        {
            access_token = _accessToken,
            fields =
                "status,date_created,date_updated,Stadt.Name,Sprache.Inhalt,Sprache.languages_id.*,Bereich.Sprache.Inhalt,Bereich.Sprache.languages_id.*,Bereich.Sprache.Bereich",
        })
          .SetQueryParam("deep[Stadt][_filter][Name][_eq]", _city)
          .SetQueryParam("filter[status]", DirectusItemStatus.Published);

        _getConfigurationUrl = "https://cms.nk-mitte.de/items/botconifguration".SetQueryParams(new
        {
            access_token = _accessToken,
            fields =
                "aktualisierungsdatumanzeigen,translations.feedbacknachricht,translations.sondermeldung,translations.referenznachricht,translations.begruessungsnachricht,translations.languages_id.*,Sprache.*",
        })
      .SetQueryParam("deep[Stadt][_filter][Name][_eq]", _city);

        _questionUrl = "https://cms.nk-mitte.de/items/fragen".SetQueryParams(new { access_token = _accessToken });


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
        await _questionUrl.PostJsonAsync(new
        {
            chatid = chatId,
            Stadt = _city,
            frage = question
        });
    }

    public async Task<DirectusQuestion[]> GetQuestionsAsync()
    {
        var questionUrl = _questionUrl
            .SetQueryParam("filter[antwort][_nnull]", "true")
            .SetQueryParam("filter[status][_eq]", "neu")
            .SetQueryParam("fields", "frage,antwort,chatid,id");
        var questions = await questionUrl.GetJsonAsync<QuestionsWrapper>();

        return questions.Data;
    }

    public async Task UpdateQuestionStatusAsync(DirectusQuestion question)
    {
        var url = $"https://cms.nk-mitte.de/items/fragen/{question.id}"
                    .SetQueryParams(new { access_token = _accessToken });
        await url.PatchJsonAsync(new { status = "beantwortet" });
    }
}