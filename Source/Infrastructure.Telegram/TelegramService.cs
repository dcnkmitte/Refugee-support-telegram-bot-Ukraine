using Dawn;
using Infrastructure.Directus;
using Infrastructure.Extensions;
using Infrastructure.Models;
using Infrastructure.Telegram.Configuration;
using Infrastructure.Telegram.Extensions;
using Infrastructure.Telegram.Models;
using Infrastructure.Telegram.Models.CommonElements;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Infrastructure.Telegram;

public class TelegramService : ITelegramService
{
    private const string specialMessagePrefix = "❗❗❗\n";
    private readonly ILogger<TelegramService> _log;
    private readonly ITelegramBotClientWrapper _botClientInternal;
    private readonly IDirectusService _directusService;
    private InlineKeyboardMarkup _helpOptionsKeyboardMarkup;
    private readonly InlineKeyboardMarkup _toMainMenuKeyboardMarkup;
    private string? _feedbackMessage;
    private string? _referenceMessage;
    private string? _specialMessage;
    private string? _welcomeMessage;
    private bool _showLastUpdadeDate;
    private Dictionary<string, Topic> responseCatalog;
    private readonly InteractiveElementBase toMainMenuButton = ToMainMenu.Create();

    public TelegramService(IOptions<TelegramConfiguration> telegramConfiguration,
                           ILogger<TelegramService> log,
                           ITelegramBotClientWrapper botClientInternal,
                           BotConfiguration botConfiguration,
                           IDirectusService directusService)
    {
        _log = log;
        _botClientInternal = botClientInternal;
        _directusService = directusService;
        Guard.Argument(telegramConfiguration.Value?.AccessToken, nameof(TelegramConfiguration.AccessToken))
            .NotEmpty("The telegram access token must be provided in the configuration.");

        _toMainMenuKeyboardMarkup = InlineKeyboardButton.WithCallbackData(toMainMenuButton.Title, toMainMenuButton.Id);

        UpdateBotConfiguration(botConfiguration);
    }

    public void UpdateBotConfiguration(BotConfiguration botConfiguration)
    {
        if (botConfiguration == null) return;
        var preferredLanguage = botConfiguration.PreferredLanguage.Name;
        var configurationArea = botConfiguration.ConfigurationContentArea;
        _feedbackMessage = configurationArea.GetIdeallyInPreferredLanguage(preferredLanguage, x => x.Feedback);
        _referenceMessage = configurationArea.GetIdeallyInPreferredLanguage(preferredLanguage, x => x.Reference);
        _specialMessage = configurationArea.GetIdeallyInPreferredLanguage(preferredLanguage, x => x.Special);
        _welcomeMessage = configurationArea.GetIdeallyInPreferredLanguage(preferredLanguage, x => x.Welcome);
        _showLastUpdadeDate = botConfiguration.ShowLastUpdadeDate;
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
        _helpOptionsKeyboardMarkup = topics.ToInlineKeyboard();
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

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"<strong>{topic.Title}</strong> \n \n {topic.ResponseBody} ");
            if (_showLastUpdadeDate)
                stringBuilder.AppendLine($"\n \n<strong>Последнее обновление: {updatedDateTime}</strong>");
            var text = stringBuilder.ToString();


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


        await PrintGoToMainMenuAsync(chatId, cancellationToken, _feedbackMessage);
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
            await PrintWelcomeMessageAsync(chatId, cancellationToken);
            await PrintMainMenuAsync(chatId, cancellationToken);
            return;
        }

        await _directusService.PostQuestionAsync(chatId, messageText);

        _log.LogInformation("Received a custom '{TextMessage}' message in chat '{ChatId}'", messageText, chatId);

        await PrintGoToMainMenuAsync(chatId, cancellationToken, _referenceMessage);
    }

    private async Task PrintWelcomeMessageAsync(long chatId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_welcomeMessage)) return;
        await _botClientInternal.SendTextMessageAsync(chatId, _welcomeMessage, cancellationToken: cancellationToken);
    }

    private async Task PrintMainMenuAsync(long chatId, CancellationToken cancellationToken)
    {
        await _botClientInternal.SendTextMessageAsync(
          chatId,
          "Что вас интересует?",
          replyMarkup: _helpOptionsKeyboardMarkup,
          cancellationToken: cancellationToken);

        if (string.IsNullOrEmpty(_specialMessage) == false)
        {
            await _botClientInternal.SendTextMessageAsync(
                chatId,
                specialMessagePrefix + _specialMessage,
                cancellationToken: cancellationToken);
        }
    }

    private async Task PrintGoToMainMenuAsync(long chatId, CancellationToken cancellationToken, string message) => await _botClientInternal.SendTextMessageAsync(
          chatId,
          message,
          replyMarkup: _toMainMenuKeyboardMarkup,
          cancellationToken: cancellationToken);

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