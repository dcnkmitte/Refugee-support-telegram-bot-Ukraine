using Infrastructure.Directus.Models;

namespace Infrastructure.Directus;

public interface IDirectusService
{
  Task<DirectusTopic[]> GetTopicsAsync();
  string PreferredLanguage { get; }
}