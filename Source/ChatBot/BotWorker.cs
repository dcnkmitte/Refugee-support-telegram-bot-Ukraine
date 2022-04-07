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
    private readonly ITelegramService _telegramService;
    private readonly IDirectusService _directusService;
    private readonly ILogger<BotWorker> _log;
    private IMapper<DirectusTopic, Topic> _topicMapper;

    public BotWorker(ITelegramService telegramService, IDirectusService directusService, ILogger<BotWorker> log, IMapper<DirectusTopic, Topic> topicMapper)
    {
        _telegramService = telegramService;
        _directusService = directusService;
        _topicMapper = topicMapper;
        _log = log;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _log.LogInformation("Start execution");

        var topics = await LoadTopicsAsync();
        await _telegramService.StartAsync(topics, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            _log.LogDebug("Checking for topic updates ...");
            try
            {
                var updatedTopics = await LoadTopicsAsync();
                _telegramService.UpdateTopics(updatedTopics);
                _log.LogDebug("Loaded update with '{TopicCount}' topics", updatedTopics.Count);
            }
            catch (Exception e)
            {
                _log.LogError("Could not refresh topics. Error: {ErrorMessage}", e.Message);
            }
        }

        _log.LogInformation("Finished execution");
    }

    private async Task<List<Topic>> LoadTopicsAsync()
    {
        var directusTopics = await _directusService.GetTopicsAsync();

        var topics = _topicMapper.Map(directusTopics).ToList();

        return topics;
    }
}