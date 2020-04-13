using System;

namespace Telegram.Bot.Instagram.Instagram.Api.Unofficial.Models
{
    public class UserCredentials
    {
        public UserCredentials(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(userName));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(password));

            UserName = userName;
            Password = password;
        }

        public string UserName { get; }
        public string Password { get; }
    }
}