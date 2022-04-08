using Infrastructure.Directus.Models;
using Infrastructure.Models;

namespace Infrastructure.Directus;

public interface IDirectusService
{
    Task<DirectusTopic[]> GetTopicsAsync();
    Task<BotConfiguration[]> GetConfigurationAsync();

}