using Dawn;
using Infrastructure.Telegram.Configuration;
using Infrastructure.Telegram.Extensions;
using Infrastructure.Telegram.Models;
using Infrastructure.Telegram.Models.CommonElements;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Infrastructure.Telegram;

public class TelegramService : ITelegramService
{
  private readonly ILogger<TelegramService> log;
  private readonly TelegramBotClient botClientInternal;
  private InlineKeyboardMarkup helpOptionsKeyboardMarkup;
  private readonly InlineKeyboardMarkup toMainMenuKeyboardMarkup;
  private Dictionary<string, Topic> responseCatalog;
  private readonly InteractiveElementBase toMainMenuButton = ToMainMenu.Create();

  public TelegramService(IOptions<TelegramConfig> configContainer, ILogger<TelegramService> log)
  {
    this.log = log;
    Guard.Argument(configContainer.Value?.AccessToken, nameof(TelegramConfig.AccessToken))
      .NotEmpty("The telegram access token must be provided in the configuration.");

    botClientInternal = new TelegramBotClient(configContainer.Value!.AccessToken);
    toMainMenuKeyboardMarkup =
      InlineKeyboardButton.WithCallbackData(toMainMenuButton.Title, toMainMenuButton.Id);
  }

  public async Task StartAsync(ICollection<Topic> topics, CancellationToken cancellationToken)
  {
    UpdateTopics(topics);

    var receiverOptions = new ReceiverOptions
    {
      AllowedUpdates = new[] { UpdateType.CallbackQuery, UpdateType.Message }
    };

    log.LogInformation("Connecting to telegram...");
    botClientInternal.StartReceiving(TryHandleUpdateAsync, HandleErrorAsync, receiverOptions,
      cancellationToken);

    var me = await botClientInternal.GetMeAsync(cancellationToken);
    log.LogInformation("Start listening for '{Username}'", me.Username);
  }

  public void UpdateTopics(ICollection<Topic> topics)
  {
    helpOptionsKeyboardMarkup = topics.ToInlineKeyboard();
    responseCatalog = topics.ToDictionary(x => x.Id, x => x);
  }

  private async Task TryHandleUpdateAsync(ITelegramBotClient botClient, Update update,
    CancellationToken cancellationToken)
  {
    try
    {
      await HandleUpdateAsync(botClient, update, cancellationToken);
    }
    catch (Exception e)
    {
      log.LogError("Cannot handle message. Error - '{Error}'", e.Message);
    }
  }

  private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
  {
    switch (update.Type)
    {
      case UpdateType.Message:
      {
        await HandleTextMessageAsync(update, cancellationToken);

        break;
      }
      case UpdateType.CallbackQuery:
      {
        await HandleButtonClickAsync(botClient, update, cancellationToken);

        break;
      }
    }
  }

  private async Task HandleButtonClickAsync(ITelegramBotClient botClient, Update update,
    CancellationToken cancellationToken)
  {
    var chatId = update.CallbackQuery!.Message!.Chat.Id;
    var topicId = update.CallbackQuery.Data;

    if (topicId == toMainMenuButton.Id)
    {
      await PrintMainMenuAsync(chatId, cancellationToken);
      return;
    }

    if (responseCatalog.TryGetValue(topicId, out var topic))
    {
      var updatedDateTime = topic.UpdatedDateTimeUtc.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
      var text =
        $"<strong>{topic.Title}</strong> \n \n {topic.ResponseBody} \n \n<strong>Последнее обновление: {updatedDateTime}</strong>";

      log.LogInformation("Request to topic '{TopicName}', topicId '{TopicId}'", topic.Title, topicId);
      await botClient.SendTextMessageAsync(
        chatId,
        text,
        ParseMode.Html,
        cancellationToken: cancellationToken);
    }
    else
    {
      log.LogWarning("Got a request to an unknown topicId '{TopicId}'", topicId);
    }

    await PrintGoToMainMenuAsync(chatId, cancellationToken);
  }

  private async Task HandleTextMessageAsync(Update update, CancellationToken cancellationToken)
  {
    var chatId = update.Message!.Chat.Id;
    var isMediaMessage = update.Message.Type != MessageType.Text;
    if (isMediaMessage)
    {
      await PrintMainMenuAsync(chatId, cancellationToken);
      return;
    }

    var messageText = update.Message.Text;
    var isStartMessage = messageText == "/start";
    if (isStartMessage)
    {
      await PrintMainMenuAsync(chatId, cancellationToken);
      return;
    }

    log.LogInformation("Received a custom '{TextMessage}' message in chat '{ChatId}'", messageText, chatId);

    await PrintGoToMainMenuAsync(chatId, cancellationToken,
      "Мы передали Ваш вопрос администраторам и постараемся добавить на него ответ в ближайшие дни. \n");
  }

  private async Task PrintMainMenuAsync(long chatId, CancellationToken cancellationToken)
  {
    await botClientInternal.SendTextMessageAsync(
      chatId,
      "Что вас интересует?",
      replyMarkup: helpOptionsKeyboardMarkup,
      cancellationToken: cancellationToken);
  }

  private async Task PrintGoToMainMenuAsync(long chatId, CancellationToken cancellationToken,
    string message = "Если информация устарела, сообщите нам ukraine@nk-mitte.de")
  {
    await botClientInternal.SendTextMessageAsync(
      chatId,
      message,
      replyMarkup: toMainMenuKeyboardMarkup,
      cancellationToken: cancellationToken);
  }

  private async Task HandleErrorAsync(ITelegramBotClient _, Exception exception, CancellationToken cancellationToken)
  {
    var errorMessage = exception switch
    {
      ApiRequestException apiRequestException
        => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
      _ => exception.ToString()
    };

    log.LogError(errorMessage);
  }
}