using Infrastructure.Directus.Models;
using Infrastructure.Models;

namespace Infrastructure.Directus;

public interface IDirectusService
{
    Task<DirectusTopic[]> GetTopicsAsync();
    Task<BotConfiguration[]> GetConfigurationAsync();
    Task PostQuestionAsync(long chatId, string? question);
    Task<DirectusQuestion[]> GetQuestionsAsync();
    Task UpdateQuestionStatusAsync(DirectusQuestion question);
}