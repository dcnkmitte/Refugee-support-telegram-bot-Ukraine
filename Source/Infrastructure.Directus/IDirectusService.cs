using Infrastructure.Directus.Models;

namespace Infrastructure.Directus;

public interface IDirectusService
{
  Task<(DirectusTopicName names, DirectusTopicBody bodies)> GetTopicsAsync();
}