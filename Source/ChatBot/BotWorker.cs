using Infrastructure.Directus;
using Infrastructure.Directus.Models;
using Infrastructure.Telegram;
using Infrastructure.Telegram.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChatBot;

public class BotWorker : BackgroundService
{
  private readonly ITelegramService telegramService;
  private readonly IDirectusService directusService;
  private readonly ILogger<BotWorker> log;

  public BotWorker(ITelegramService telegramService, IDirectusService directusService, ILogger<BotWorker> log)
  {
    this.telegramService = telegramService;
    this.directusService = directusService;
    this.log = log;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    this.log.LogInformation("Start execution");

    var topics = await this.LoadTopicsAsync();
    await this.telegramService.StartAsync(topics, stoppingToken);

    while (!stoppingToken.IsCancellationRequested)
    {
      await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
      this.log.LogDebug("Checking for topic updates ...");
      try
      {
        var updatedTopics = await this.LoadTopicsAsync();
        this.telegramService.UpdateTopics(updatedTopics);
        this.log.LogDebug("Loaded update with '{TopicCount}' topics", updatedTopics.Count);
      }
      catch (Exception e)
      {
        this.log.LogError("Could not refresh topics. Error: {ErrorMessage}", e.Message);
      }
    }

    this.log.LogInformation("Finished execution");
  }

  private async Task<List<Topic>> LoadTopicsAsync()
  {
    var directusTopics = await this.directusService.GetTopicsAsync();
    var topics = this.ConvertToTopics(directusTopics);
    return topics;
  }

  private List<Topic> ConvertToTopics(DirectusTopic[] directusTopics)
  {
    var preferredLanguage = this.directusService.PreferredLanguage;
    var result = new List<Topic>();

    foreach (var directusTopic in directusTopics)
    {
      var isTopicNamePresentInPreferredLanguage =
        directusTopic.DetailsTopicNameArea.MultiLanguageBody.Any(x => x.Language.Name.Equals(preferredLanguage));

      var isTopicContentPresentInPreferredLanguage =
        directusTopic.DetailsTopicContentArea.Any(x => x.Language.Name.Equals(preferredLanguage));

      var topicName = isTopicNamePresentInPreferredLanguage
        ? directusTopic.DetailsTopicNameArea.MultiLanguageBody.First(x => x.Language.Name.Equals(preferredLanguage))
          .TopicName
        : directusTopic.DetailsTopicNameArea.MultiLanguageBody.FirstOrDefault()?.TopicName;

      if (topicName == null) continue;

      var topicContent = isTopicContentPresentInPreferredLanguage
        ? directusTopic.DetailsTopicContentArea.First(x => x.Language.Name.Equals(preferredLanguage)).TopicContent
        : directusTopic.DetailsTopicContentArea.FirstOrDefault()?.TopicContent;

      if (topicContent == null) continue;

      var updatedDateTimeUtc = directusTopic.DateUpdated ?? directusTopic.DateCreated;

      result.Add(new Topic(topicName, topicContent, updatedDateTimeUtc));
    }

    return result;
  }
}