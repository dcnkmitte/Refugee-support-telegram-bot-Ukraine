using Infrastructure.Directus;
using Infrastructure.Directus.Models;
using Infrastructure.Telegram;
using Infrastructure.Telegram.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChatBot;

public class BotWorker : BackgroundService
{
  private readonly ITelegramService telegramService;
  private readonly IDirectusService directusService;
  private readonly ILogger<BotWorker> log;

  public BotWorker(ITelegramService telegramService, IDirectusService directusService, ILogger<BotWorker> log)
  {
    this.telegramService = telegramService;
    this.directusService = directusService;
    this.log = log;
  }

  // new List<Topic>()
  // {
  //   new Topic("Проживання 🏠", "Проживання\n https://nkmitte.visualstudio.com/Refugee%20support%20telegram%20bot%20Ukraine"),
  //   new Topic("Новини", "Новиниsjif0wküeofmwpikopwefpik\nwpkp wkp wpok wpo\ne kpwok vpowe \n oisajiowiofnwio"),
  //   new Topic("Медична допомога 🌡", "Медична допомога\nwpkp wkp wpok wpo\ne kpwok vpowe \n oisajiowiofnwio"),
  //   new Topic("Оформление виз 📄", "Оформлення док\nументівsjif0wküeofmwpikopwefpikwpkp wkp wpok wpoe kpwok vpowe \n oisajiowiofnwio"),
  //   new Topic("Связь 📞", "Бесплатные звонки в Украину из городских таксофонов \n Дойче Телеком предоставил возможность звонить в Украину бесплатно из своих телефонных  будок. \n Звонить на телефоны с кодом Украины 00380 (+380) можно без жетонов и без телефонных карт."),
  //   new Topic("Інша інформація", "Інша інформаціяsjif\n0wküeofmwpikopwefpikw\npkp wkp wpok wpoe kpwok vpowe \n oisajiowiofnwio"),
  // }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    this.log.LogInformation("Start execution");

    var topics = await this.LoadTopicsAsync();
    await this.telegramService.StartAsync(topics, stoppingToken);

    while (!stoppingToken.IsCancellationRequested)
    {
      await Task.Delay(20000, stoppingToken);
      this.log.LogInformation("Checking for topic updates ...");
      var updatedTopics = await this.LoadTopicsAsync();
      this.telegramService.UpdateTopics(updatedTopics);
      this.log.LogInformation($"Loaded {updatedTopics.Count} topics");
    }

    this.log.LogInformation("Finished execution");
  }

  private async Task<List<Topic>> LoadTopicsAsync()
  {
    var directusTopics = await this.directusService.GetTopicsAsync();
    var topics = this.ConvertToTopics(directusTopics);
    return topics;
  }

  private List<Topic> ConvertToTopics(DirectusTopic[] directusTopics)
  {
    var preferredLanguage = this.directusService.PreferredLanguage;
    var result = new List<Topic>();

    foreach (var directusTopic in directusTopics)
    {
      var isTopicNamePresentInPreferredLanguage =
        directusTopic.DetailsTopicNameArea.MultiLanguageBody.Any(x => x.Language.Name.Equals(preferredLanguage));

      var isTopicContentPresentInPreferredLanguage =
        directusTopic.DetailsTopicContentArea.Any(x => x.Language.Name.Equals(preferredLanguage));

      var topicName = isTopicContentPresentInPreferredLanguage
        ? directusTopic.DetailsTopicNameArea.MultiLanguageBody.First(x => x.Language.Name.Equals(preferredLanguage))
          .TopicName
        : directusTopic.DetailsTopicNameArea.MultiLanguageBody.FirstOrDefault()?.TopicName;

      if (topicName == null) continue;

      var topicContent = isTopicContentPresentInPreferredLanguage
        ? directusTopic.DetailsTopicContentArea.First(x => x.Language.Name.Equals(preferredLanguage)).TopicContent
        : directusTopic.DetailsTopicContentArea.FirstOrDefault()?.TopicContent;

      if (topicContent == null) continue;

      result.Add(new Topic(topicName, topicContent));
    }

    return result;
  }
}