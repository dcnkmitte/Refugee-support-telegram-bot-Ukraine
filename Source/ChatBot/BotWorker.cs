using ChatBot.Mappers;
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
  private IMapper<DirectusTopic, Topic> topicMapper;

  public BotWorker(ITelegramService telegramService, IDirectusService directusService, ILogger<BotWorker> log)
  {
    this.telegramService = telegramService;
    this.directusService = directusService;
    this.topicMapper = new DirectusTopicToTopicMapper(directusService.PreferredLanguage);
    this.log = log;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    this.log.LogInformation("Start execution");

    var topics = await this.LoadTopicsAsync();
    await this.telegramService.StartAsync(topics, stoppingToken);

    while (!stoppingToken.IsCancellationRequested)
    {
      await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
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

    var topics = this.topicMapper.Map(directusTopics).ToList();

    return topics;
  }
}