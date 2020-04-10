using Moq;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Instagram.Telegram;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Xunit;
using Message = Telegram.Bot.Instagram.Telegram.Message;

namespace Telegram.Bot.Instagram.Tests.Telegram
{
    public class BaseTelegramBotTest
    {
        private readonly ITelegramBotClient _fakeTelegramBotClient = new Mock<ITelegramBotClient>().Object;

        [Fact]
        public void It_Implements_ITelegramBot()
        {
            ITelegramBotClient telegramBotClient = new Mock<ITelegramBotClient>().Object;

            // ReSharper disable once SuggestVarOrType_SimpleTypes
            ITelegramBot telegramBot = new BaseTelegramBot(telegramBotClient);

            Assert.NotNull(telegramBot);
        }

        [Fact]
        public void Ctor_NullTelegramBotClient_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new BaseTelegramBot(null));
        }

        [Fact]
        public void StartReceiving_ItStartsReceivingFromTheInnerClient()
        {
            var telegramBotClientMock = new Mock<ITelegramBotClient>();
            ITelegramBot telegramBot = new BaseTelegramBot(telegramBotClientMock.Object);

            telegramBot.StartReceiving();

            telegramBotClientMock.Verify(i => i.StartReceiving(null, default), Times.Once);
        }

        [Fact]
        public async Task SendMessageAsync_NullMessage_ItThrowsArgumentNullException()
        {
            ITelegramBot telegramBot = new BaseTelegramBot(_fakeTelegramBotClient);

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await telegramBot.SendMessageAsync(default));
        }

        [Fact]
        public async Task SendMessageAsync_NotNullMessage_ItSendsTheMessage()
        {
            var message = new Message
            {
                ChatId = 1,
                Text = "Hello World"
            };
            var telegramBotClientMock = new Mock<ITelegramBotClient>();
            ITelegramBot telegramBot = new BaseTelegramBot(telegramBotClientMock.Object);

            await telegramBot.SendMessageAsync(message);

            telegramBotClientMock.Verify(i => i.SendTextMessageAsync(
                It.Is<ChatId>(p => p.Identifier == message.ChatId),
                It.Is<string>(p => p == message.Text),
                ParseMode.Default, false, false, 0, null, default),
                Times.Once);
        }

        [Fact]
        public void SetMessageHandler_NullMessageHandler_ItSetIt()
        {
            var telegramBot = new BaseTelegramBot(_fakeTelegramBotClient);

            telegramBot.SetMessageHandler(default);

            Assert.Equal(default, telegramBot.MessageHandler);
        }

        [Fact]
        public void SetMessageHandler_NotNullMessageHandler_ItSetIt()
        {
            var telegramBotClientMock = new Mock<ITelegramBotClient>();
            var telegramBot = new BaseTelegramBot(telegramBotClientMock.Object);

            IMessageHandler fakeMssageHandler = new Mock<IMessageHandler>().Object;

            telegramBot.SetMessageHandler(fakeMssageHandler);

            Assert.Equal(fakeMssageHandler, telegramBot.MessageHandler);
        }
    }
}