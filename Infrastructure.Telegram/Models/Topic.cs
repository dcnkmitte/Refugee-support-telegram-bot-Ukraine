namespace Infrastructure.Telegram.Models;

public class Topic : InteractiveElementBase
{
  public Topic(string title, string responseBody) : base(title)
  {
    this.ResponseBody = responseBody;
  }

  public string ResponseBody { get; }
}