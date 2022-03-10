using Infrastructure.Telegram;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChatBot;

public class BotWorker : BackgroundService
{
  private readonly ITelegramService telegramService;
  private readonly ILogger<BotWorker> log;

  public BotWorker(ITelegramService telegramService, ILogger<BotWorker> log)
  {
    this.telegramService = telegramService;
    this.log = log;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    this.log.LogInformation("Start executing");

    await Task.Delay(1000, stoppingToken);

    this.log.LogInformation("Finished execution");
  }
}