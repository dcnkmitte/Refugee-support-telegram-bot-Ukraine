using System.Collections.ObjectModel;
using Infrastructure.Telegram.Extensions;
using Telegram.Bot.Types.ReplyMarkups;

namespace Infrastructure.Telegram.Models;

internal class KeyboardRow
{
  private const int MaxLength = 30;
  private int length;

  private readonly List<InlineKeyboardButton> buttons = new();

  public bool CanAdd(Topic topic) => this.buttons.Empty() || this.IsTotalWithWithinMaxLength(topic);

  private bool IsTotalWithWithinMaxLength(InteractiveElementBase topic)
  {
    return this.length + topic.TitleWidth <= MaxLength;
  }

  private static bool IsTitleWiderThanHalfMaxLength(InteractiveElementBase topic)
  {
    return topic.TitleWidth * 2 > MaxLength;
  }

  public void Add(Topic topic)
  {
    this.buttons.Add(InlineKeyboardButton.WithCallbackData(topic.Title, topic.Id));
    if (IsTitleWiderThanHalfMaxLength(topic))
    {
      this.length = MaxLength;
    }
    else
    {
      this.length += topic.TitleWidth;
    }
  }

  public ReadOnlyCollection<InlineKeyboardButton> GetButtons() => this.buttons.AsReadOnly();
}