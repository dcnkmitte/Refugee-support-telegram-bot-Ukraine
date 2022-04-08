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
    private readonly ILogger<TelegramService> _log;
    private readonly ITelegramBotClientWrapper _botClientInternal;
    private InlineKeyboardMarkup helpOptionsKeyboardMarkup;
    private readonly InlineKeyboardMarkup _toMainMenuKeyboardMarkup;
    private Dictionary<string, Topic> responseCatalog;
    private readonly InteractiveElementBase toMainMenuButton = ToMainMenu.Create();

    public TelegramService(IOptions<TelegramConfiguration> configContainer, ILogger<TelegramService> log, ITelegramBotClientWrapper botClientInternal)
    {
        _log = log;
        _botClientInternal = botClientInternal;
        Guard.Argument(configContainer.Value?.AccessToken, nameof(TelegramConfiguration.AccessToken))
      .NotEmpty("The telegram access token must be provided in the configuration.");

        _toMainMenuKeyboardMarkup =
          InlineKeyboardButton.WithCallbackData(toMainMenuButton.Title, toMainMenuButton.Id);
    }

    public async Task StartAsync(ICollection<Topic> topics, CancellationToken cancellationToken)
    {
        UpdateTopics(topics);

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[] { UpdateType.CallbackQuery, UpdateType.Message }
        };

        _log.LogInformation("Connecting to telegram...");
        _botClientInternal.StartReceiving(TryHandleUpdateAsync, HandleErrorAsync, receiverOptions,
          cancellationToken);

        var me = await _botClientInternal.GetMeAsync(cancellationToken);
        _log.LogInformation("Start listening for '{Username}'", me.Username);
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
            _log.LogError("Cannot handle message. Error - '{Error}'", e.Message);
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

            _log.LogInformation("Request to topic '{TopicName}', topicId '{TopicId}'", topic.Title, topicId);
            await botClient.SendTextMessageAsync(
              chatId,
              text,
              ParseMode.Html,
              cancellationToken: cancellationToken);
        }
        else
        {
            _log.LogWarning("Got a request to an unknown topicId '{TopicId}'", topicId);
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

        _log.LogInformation("Received a custom '{TextMessage}' message in chat '{ChatId}'", messageText, chatId);


        // TODO: Get the message from Backend
        await PrintGoToMainMenuAsync(chatId, cancellationToken,
          "Мы передали Ваш вопрос администраторам и постараемся добавить на него ответ в ближайшие дни. \n");
    }

    private async Task PrintMainMenuAsync(long chatId, CancellationToken cancellationToken)
    {
        await _botClientInternal.SendTextMessageAsync(
          chatId,
          "Что вас интересует?",
          replyMarkup: helpOptionsKeyboardMarkup,
          cancellationToken: cancellationToken);
    }

    private async Task PrintGoToMainMenuAsync(long chatId, CancellationToken cancellationToken,
      string message = "Если информация устарела, сообщите нам ukraine@nk-mitte.de")
    {
        await _botClientInternal.SendTextMessageAsync(
          chatId,
          message,
          replyMarkup: _toMainMenuKeyboardMarkup,
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

        _log.LogError(errorMessage);
    }
}