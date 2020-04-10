using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Instagram.Instagram.Api.Unofficial.Models;

namespace Telegram.Bot.Instagram.Instagram.Api.Unofficial
{
    public class UnofficialInstagramApi
    {
        public async Task<bool> LoginAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<InstagramMedia>> GetMediaFromPost(Uri uri)
        {
            throw new NotImplementedException();
        }

        public async Task<string[]> DownloadMediaFromPost(Uri uri)
        {
            throw new NotImplementedException();

        }
    }
}