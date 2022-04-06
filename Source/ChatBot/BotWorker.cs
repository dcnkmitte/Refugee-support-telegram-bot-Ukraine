﻿using ChatBot.Mappers;
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
    topicMapper = new DirectusTopicToTopicMapper(directusService.PreferredLanguage);
    this.log = log;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    log.LogInformation("Start execution");

    var topics = await LoadTopicsAsync();
    await telegramService.StartAsync(topics, stoppingToken);

    while (!stoppingToken.IsCancellationRequested)
    {
      await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
      log.LogDebug("Checking for topic updates ...");
      try
      {
        var updatedTopics = await LoadTopicsAsync();
        telegramService.UpdateTopics(updatedTopics);
        log.LogDebug("Loaded update with '{TopicCount}' topics", updatedTopics.Count);
      }
      catch (Exception e)
      {
        log.LogError("Could not refresh topics. Error: {ErrorMessage}", e.Message);
      }
    }

    log.LogInformation("Finished execution");
  }

  private async Task<List<Topic>> LoadTopicsAsync()
  {
    var directusTopics = await directusService.GetTopicsAsync();

    var topics = topicMapper.Map(directusTopics).ToList();

    return topics;
  }
}