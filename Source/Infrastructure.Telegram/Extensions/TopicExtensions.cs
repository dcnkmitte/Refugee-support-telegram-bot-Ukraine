using Infrastructure.Telegram.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace Infrastructure.Telegram.Extensions;

public static class TopicExtensions
{
    public static InlineKeyboardMarkup ToInlineKeyboard(this IEnumerable<Topic> topics)
    {
        var rows = new List<KeyboardRow>();
        var currentRow = new KeyboardRow();

        foreach (var topic in topics)
        {
            if (currentRow.CanAdd(topic))
            {
                currentRow.Add(topic);
            }
            else
            {
                rows.Add(currentRow);
                currentRow = new KeyboardRow();
                currentRow.Add(topic);
            }
        }

        rows.Add(currentRow);

        var result = new InlineKeyboardMarkup(rows.Select(x => x.GetButtons()));
        return result;
    }
}