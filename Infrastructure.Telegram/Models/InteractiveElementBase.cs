using Infrastructure.Extensions;

namespace Infrastructure.Telegram.Models;

public abstract class InteractiveElementBase
{
  protected InteractiveElementBase(string title)
  {
    this.Title = title;
    this.Id = title.GetMd5Hash();
    this.TitleWidth = title.Length;
  }

  public int TitleWidth { get; }
  public string Title { get; }
  public string Id { get; }
}