using System.Threading.Tasks;

namespace Telegram.Bot.Instagram.Telegram
{
    public interface ITelegramBot
    {
        void StartReceiving();

        void SetMessageHandler(IMessageHandler messageHandler);

        Task SendMessageAsync(Message message);
    }
}