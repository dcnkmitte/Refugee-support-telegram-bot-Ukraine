using NUnit.Framework;
using ChatBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Telegram;
using Moq;
using Infrastructure.Directus;
using Microsoft.Extensions.Logging;

namespace ChatBot.Tests
{
    [TestFixture()]
    public class BotWorkerTests
    {
        [Test()]
        public void When_BotWorkerIsCreated_Then_NoExceptionOccours()
        {
            //arrange
            var telegramServiceMock = new Mock<ITelegramService>();
            var directusServiceMock = new Mock<IDirectusService>();
            var loggerMock = new Mock<ILogger<BotWorker>>();

            //act
            _ = new BotWorker(telegramServiceMock.Object,
                directusServiceMock.Object, loggerMock.Object);

            //assert
            Assert.Pass();
        }
    }
}