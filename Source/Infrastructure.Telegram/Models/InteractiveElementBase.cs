using Infrastructure.Extensions;

namespace Infrastructure.Telegram.Models;

public abstract class InteractiveElementBase
{
  protected InteractiveElementBase(string title)
  {
    Title = title;
    Id = title.GetMd5Hash();
    TitleWidth = title.Length;
  }

  public int TitleWidth { get; }
  public string Title { get; }
  public string Id { get; }
}