using Infrastructure.Telegram.Models;

namespace Infrastructure.Telegram;

public interface ITelegramService
{
  Task StartAsync(ICollection<Topic> topics, CancellationToken cancellationToken);
  void UpdateTopics(ICollection<Topic> topics);
}