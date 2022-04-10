using Infrastructure.Telegram;
using Infrastructure.Telegram.Configuration;
using Infrastructure.Telegram.Extensions;
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
        var botClientInternalMock = new Mock<ITelegramBotClientWrapper>();
        var telegramUser = new User();
        botClientInternalMock.Setup(x => x.GetMeAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User() { FirstName = "bot" });
        var topics = new Collection<Topic>
        {
            new Topic("title", "body", System.DateTime.UtcNow)
        };
        var telegramService = new TelegramService(configContainerMock.Object, logMock.Object, botClientInternalMock.Object);

        //act
        await telegramService.StartAsync(topics, CancellationToken.None);

        //assert
        botClientInternalMock.Verify(x => x.StartReceiving(It.IsAny<Func<ITelegramBotClient, Update, CancellationToken, Task>>(),
            It.IsAny<Func<ITelegramBotClient, Exception, CancellationToken, Task>>(),
           It.IsAny<ReceiverOptions?>(),
           It.IsAny<CancellationToken>()), Times.Once);

    }
}