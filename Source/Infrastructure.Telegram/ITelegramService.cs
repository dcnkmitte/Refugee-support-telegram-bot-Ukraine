using Infrastructure.Directus.Models;
using Infrastructure.Models;
using Infrastructure.Telegram.Models;

namespace Infrastructure.Telegram;

public interface ITelegramService
{
    Task StartAsync(ICollection<Topic> topics, CancellationToken cancellationToken);
    void UpdateTopics(ICollection<Topic> topics);
    void UpdateBotConfiguration(BotConfiguration botConfiguration);
    Task SendAnswerToUserAsync(DirectusQuestion question, CancellationToken cancellationToken);
}