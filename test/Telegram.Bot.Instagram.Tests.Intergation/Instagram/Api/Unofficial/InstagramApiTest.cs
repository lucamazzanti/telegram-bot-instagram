using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Instagram.Instagram.Api.Unofficial;
using Xunit;

namespace Telegram.Bot.Instagram.Tests.Intergation.Instagram.Api.Unofficial
{
    public class InstagramApiTest
    {
        [Fact]
        public async Task CanExtractMediaFromPublicPost()
        {
            //extract an url post from a sample tag page
            //https://www.instagram.com/explore/tags/art
            //
            var publicPost = new Uri("https://www.instagram.com/p/B-6Tug4h_cY/");

            using var clientHandler = new HttpClientHandler();
            var api = new InstagramApi(clientHandler);
            var media = await api.GetMediaFromPostAsync(publicPost);
            Assert.NotEmpty(media);
        }
    }
}