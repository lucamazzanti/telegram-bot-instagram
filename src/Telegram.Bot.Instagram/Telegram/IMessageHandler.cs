namespace Telegram.Bot.Instagram.Telegram
{
    public interface IMessageHandler
    {
        void HandleMessage(Message message);
    }
}