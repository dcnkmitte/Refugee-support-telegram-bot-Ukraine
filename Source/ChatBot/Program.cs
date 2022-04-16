using ChatBot;
using ChatBot.Mappers;
using Infrastructure.Directus;
using Infrastructure.Directus.Configuration;
using Infrastructure.Directus.Models;
using Infrastructure.Models;
using Infrastructure.Telegram;
using Infrastructure.Telegram.Configuration;
using Infrastructure.Telegram.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Telegram.Bot;

const string seqLoggingSink = "http://seq:5341";

Console.OutputEncoding = System.Text.Encoding.UTF8;
Log.Logger = new LoggerConfiguration()
  .MinimumLevel.Debug()
  .Enrich.FromLogContext()
  .Enrich.WithAssemblyName()
  .Enrich.WithMachineName()
  .WriteTo.Seq(seqLoggingSink)
  .WriteTo.Console()
  .CreateLogger();


try
{
    Log.Information("Starting host");
    CreateHostBuilder(args).Build().Run();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

static IHostBuilder CreateHostBuilder(string[] args)
{
    return Host.CreateDefaultBuilder(args)
.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    config.AddUserSecrets<Program>();
})

.ConfigureServices((hostContext, services) =>
{
    services.AddHostedService<BotWorker>();
    services.Configure<TelegramConfiguration>(hostContext.Configuration.GetSection("Telegram"));
    services.Configure<DirectusConfiguration>(hostContext.Configuration.GetSection("Directus"));
    services.AddTransient<ITelegramService, TelegramService>();
    services.AddTransient<IDirectusService, DirectusService>();
    services.AddSingleton(x => x.GetRequiredService<IDirectusService>().GetConfigurationAsync().GetAwaiter().GetResult().First());
    services.AddSingleton<ITelegramBotClient>(x =>
    new TelegramBotClient(hostContext.Configuration.GetSection("Telegram:AccessToken").Value));
    services.AddSingleton<ITelegramBotClientWrapper, TelegramBotClientWrapper>();
    services.AddTransient<IMapper<DirectusTopic, Topic>>(x =>
    new DirectusTopicToTopicMapper(x.GetRequiredService<BotConfiguration>().PreferredLanguage.Name));
})
.UseSerilog();
}