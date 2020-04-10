using System;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
// ReSharper disable StringLiteralTypo

namespace Telegram.Bot.Instagram.Telegram
{
    public class BaseTelegramBot : ITelegramBot
    {
        private readonly ITelegramBotClient _telegramBotClient;
        public IMessageHandler MessageHandler { get; private set;  }

        public BaseTelegramBot(ITelegramBotClient telegramBotClient)
        {
            _telegramBotClient = telegramBotClient ?? throw new ArgumentNullException(nameof(telegramBotClient));
            _telegramBotClient.OnMessage += OnMessage;
        }

        public void StartReceiving()
        {
            _telegramBotClient.StartReceiving();
        }

        public void SetMessageHandler(IMessageHandler messageHandler)
        {
            MessageHandler = messageHandler;
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            var message = new Message
            {
                ChatId = e.Message.Chat.Id,
                Text = e.Message.Text
            };
            MessageHandler?.HandleMessage(message);
        }

        public async Task SendMessageAsync(Message message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            await _telegramBotClient.SendTextMessageAsync(new ChatId(message.ChatId), message.Text);
        }
    }
}