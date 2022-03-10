using Dawn;
using Infrastructure.Telegram.Configuration;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace Infrastructure.Telegram;

public class TelegramService : ITelegramService
{
  private readonly TelegramConfig config;
  private readonly TelegramBotClient botClient;

  public TelegramService(IOptions<TelegramConfig> configContainer)
  {
    this.config = configContainer.Value;
    Guard.Argument(configContainer.Value?.AccessToken, nameof(TelegramConfig.AccessToken))
      .NotEmpty("The telegram access token must be provided in the configuration.");

    this.botClient = new TelegramBotClient(this.config.AccessToken);
  }
}