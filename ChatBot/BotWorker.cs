using Infrastructure.Directus;
using Infrastructure.Directus.Models;
using Infrastructure.Telegram;
using Infrastructure.Telegram.Extensions;
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

  private List<Topic> ConvertToTopics(
    (DirectusTopicName namesContainer, DirectusTopicBody bodiesContainer) directusTopics)
  {
    var names = directusTopics.namesContainer.Data;
    var bodies = directusTopics.bodiesContainer.Data;

    var results = new List<Topic>();
    foreach (var name in names.Where(x => x.Status == DirectusItemStatus.Published))
    {
      var matchingBodies = bodies.Where(x => x.Status == DirectusItemStatus.Published && x.Area == name.Id).ToList();
      if (matchingBodies.Empty()) continue;

      var topic = new Topic(name.Name, string.Join("\n \n", matchingBodies.Select(x => x.Content)));
      results.Add(topic);
    }

    return results;
  }
}