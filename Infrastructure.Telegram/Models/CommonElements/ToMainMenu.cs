namespace Infrastructure.Telegram.Models.CommonElements;

public class ToMainMenu
{
  public static InteractiveElementBase Create() => new NavigationButton("На главное меню 🔍");
}