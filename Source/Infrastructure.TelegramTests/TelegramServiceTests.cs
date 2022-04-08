using Infrastructure.Directus;
using Infrastructure.Models;
using Infrastructure.Telegram;
using Infrastructure.Telegram.Configuration;
using Infrastructure.Telegram.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace Infrastructure.TelegramTests;
public class TelegramServiceTests
{



    [Test]
    public async Task When_StartIsCalled_Then_StartRecivingIsCalled()
    {
        //arrange
        var configContainerMock = new Mock<IOptions<TelegramConfiguration>>();
        var logMock = new Mock<ILogger<TelegramService>>();
        var directusServiceMock = new Mock<IDirectusService>();
        var botClientInternalMock = new Mock<ITelegramBotClientWrapper>();
        var botConfiguration = new BotConfiguration()
        {
            PreferredLanguage = new DirectusLanguage() { Name = "Russisch" },
            ShowLastUpdadeDate = true,
            ConfigurationContentArea = new List<BotConfigurationContentArea>() {
        new BotConfigurationContentArea(){
            Feedback="feedback",
            Language=new DirectusLanguage(){Name="Russisch"},
            Reference="reference",
            Special="special",
            Welcome="welcome" }  }
        };
        var telegramUser = new User();
        botClientInternalMock.Setup(x => x.GetMeAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User() { FirstName = "bot" });
        var topics = new Collection<Topic>
        {
            new Topic("title", "body", System.DateTime.UtcNow)
        };

        var telegramService = new TelegramService(configContainerMock.Object, logMock.Object, botClientInternalMock.Object, botConfiguration,directusServiceMock.Object);

        //act
        await telegramService.StartAsync(topics, CancellationToken.None);

        //assert
        botClientInternalMock.Verify(x => x.StartReceiving(It.IsAny<Func<ITelegramBotClient, Update, CancellationToken, Task>>(),
            It.IsAny<Func<ITelegramBotClient, Exception, CancellationToken, Task>>(),
           It.IsAny<ReceiverOptions?>(),
           It.IsAny<CancellationToken>()), Times.Once);

    }
}