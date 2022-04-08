using ChatBot;
using ChatBot.Mappers;
using Infrastructure.Directus;
using Infrastructure.Directus.Models;
using Infrastructure.Telegram;
using Infrastructure.Telegram.Models;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace ChatBotTests
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
            var topicMapperMock = new Mock<IMapper<DirectusTopic, Topic>>();

            //act
            _ = new BotWorker(telegramServiceMock.Object,
                directusServiceMock.Object, loggerMock.Object, topicMapperMock.Object);

            //assert
            Assert.Pass();
        }
    }
}