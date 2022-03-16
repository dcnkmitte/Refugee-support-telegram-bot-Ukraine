using ChatBot;
using Infrastructure.Directus;
using Infrastructure.Directus.Configuration;
using Infrastructure.Telegram;
using Infrastructure.Telegram.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

CreateHostBuilder(args).Build().Run();

static IHostBuilder CreateHostBuilder(string[] args) =>
  Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
      services.AddHostedService<BotWorker>();
      services.Configure<TelegramConfig>(hostContext.Configuration.GetSection("Telegram"));
      services.Configure<DirectusConfig>(hostContext.Configuration.GetSection("Directus"));
      services.AddTransient<ITelegramService, TelegramService>();
      services.AddTransient<IDirectusService, DirectusService>();
    });