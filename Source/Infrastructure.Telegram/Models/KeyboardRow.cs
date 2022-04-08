using System.Collections.ObjectModel;
using Infrastructure.Telegram.Extensions;
using Telegram.Bot.Types.ReplyMarkups;

namespace Infrastructure.Telegram.Models;

internal class KeyboardRow
{
  private const int MaxLength = 30;
  private int length;

  private readonly List<InlineKeyboardButton> buttons = new();

  public bool CanAdd(Topic topic) => buttons.IsEmpty() || IsTotalWithSmallerThanMaxLength(topic);

  private bool IsTotalWithSmallerThanMaxLength(InteractiveElementBase topic)
  {
    return length + topic.TitleWidth <= MaxLength;
  }

  private static bool IsTitleWiderThanHalfMaxLength(InteractiveElementBase topic)
  {
    return topic.TitleWidth * 2 > MaxLength;
  }

  public void Add(Topic topic)
  {
    buttons.Add(InlineKeyboardButton.WithCallbackData(topic.Title, topic.Id));
    if (IsTitleWiderThanHalfMaxLength(topic))
    {
      length = MaxLength;
    }
    else
    {
      length += topic.TitleWidth;
    }
  }

  public ReadOnlyCollection<InlineKeyboardButton> GetButtons() => buttons.AsReadOnly();
}