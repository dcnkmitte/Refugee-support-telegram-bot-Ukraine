namespace Infrastructure.Telegram.Models;

public class Topic : InteractiveElementBase
{
  public Topic(string title, string responseBody, DateTime updatedDateTimeUtc) : base(title)
  {
    ResponseBody = responseBody;
    UpdatedDateTimeUtc = updatedDateTimeUtc;
  }

  public string ResponseBody { get; }
  public DateTime UpdatedDateTimeUtc { get; }
}