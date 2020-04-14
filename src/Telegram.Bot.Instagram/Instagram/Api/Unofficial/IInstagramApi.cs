using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Instagram.Instagram.Api.Unofficial.Models;

namespace Telegram.Bot.Instagram.Instagram.Api.Unofficial
{
    public interface IInstagramApi
    {
        string UserName { get; }
        bool IsAuthenticated { get; }
        Task<bool> LoginAsync(UserCredentials userCredentials);
        void LoadSession(string sessionFile);
        void SaveSession(string sessionFile);
        Task<IEnumerable<InstagramMedia>> GetMediaFromPostAsync(Uri url);
        Task<string[]> DownloadMediaFromPostAsync(Uri url, string outputDirectory);
        void Dispose();
    }
}