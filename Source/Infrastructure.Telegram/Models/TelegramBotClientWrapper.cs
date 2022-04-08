using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Infrastructure.Telegram.Models;

public class TelegramBotClientWrapper : ITelegramBotClientWrapper
{
    private readonly ITelegramBotClient _telegramBotClient;

    public TelegramBotClientWrapper(ITelegramBotClient telegramBotClient)
    {
        _telegramBotClient = telegramBotClient;
    }
    public async Task<User> GetMeAsync(CancellationToken cancellationToken = default) =>
        await _telegramBotClient.GetMeAsync(cancellationToken);

    public async Task<Message> SendTextMessageAsync(ChatId chatId, string text, ParseMode? parseMode = null, IEnumerable<MessageEntity>? entities = null, bool? disableWebPagePreview = null, bool? disableNotification = null, int? replyToMessageId = null, bool? allowSendingWithoutReply = null, IReplyMarkup? replyMarkup = null, CancellationToken cancellationToken = default) =>
        await _telegramBotClient.SendTextMessageAsync(chatId, text, parseMode, entities, disableWebPagePreview, disableNotification);

    public void StartReceiving(Func<ITelegramBotClient, Update, CancellationToken, Task> updateHandler, Func<ITelegramBotClient, Exception, CancellationToken, Task> errorHandler, ReceiverOptions? receiverOptions = null, CancellationToken cancellationToken = default) =>
        _telegramBotClient.StartReceiving(updateHandler, errorHandler, receiverOptions, cancellationToken);
}