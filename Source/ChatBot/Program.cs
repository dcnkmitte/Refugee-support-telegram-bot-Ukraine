using ChatBot;
using Infrastructure.Directus;
using Infrastructure.Directus.Configuration;
using Infrastructure.Telegram;
using Infrastructure.Telegram.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

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

static IHostBuilder CreateHostBuilder(string[] args) =>
  Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
      services.AddHostedService<BotWorker>();
      services.Configure<TelegramConfig>(hostContext.Configuration.GetSection("Telegram"));
      services.Configure<DirectusConfig>(hostContext.Configuration.GetSection("Directus"));
      services.AddTransient<ITelegramService, TelegramService>();
      services.AddTransient<IDirectusService, DirectusService>();
    })
    .UseSerilog();