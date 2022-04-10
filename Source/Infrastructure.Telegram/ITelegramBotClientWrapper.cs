using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Infrastructure.Telegram;

public interface ITelegramBotClientWrapper
{
    public void StartReceiving(Func<ITelegramBotClient, Update, CancellationToken, Task> updateHandler, Func<ITelegramBotClient, Exception, CancellationToken, Task> errorHandler, ReceiverOptions? receiverOptions = null, CancellationToken cancellationToken = default(CancellationToken));
    public Task<User> GetMeAsync(CancellationToken cancellationToken = default);
    public Task<Message> SendTextMessageAsync(ChatId chatId, string text, ParseMode? parseMode = null, IEnumerable<MessageEntity>? entities = null, bool? disableWebPagePreview = null, bool? disableNotification = null, int? replyToMessageId = null, bool? allowSendingWithoutReply = null, IReplyMarkup? replyMarkup = null, CancellationToken cancellationToken = default);
}
