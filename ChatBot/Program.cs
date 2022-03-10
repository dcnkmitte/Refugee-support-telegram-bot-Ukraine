using ChatBot;
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
      services.AddTransient<ITelegramService, TelegramService>();
    });