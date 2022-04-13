using Infrastructure.Models;

namespace Infrastructure;

public interface IMultilanguage
{
    DirectusLanguage Language { get; }
}