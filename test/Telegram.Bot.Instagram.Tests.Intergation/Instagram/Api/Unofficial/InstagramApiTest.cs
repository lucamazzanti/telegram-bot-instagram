using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Instagram.Instagram.Api.Unofficial;
using Telegram.Bot.Instagram.Instagram.Api.Unofficial.Models;
using Xunit;
// ReSharper disable StringLiteralTypo
// ReSharper disable AssignNullToNotNullAttribute

namespace Telegram.Bot.Instagram.Tests.Intergation.Instagram.Api.Unofficial
{
    public class InstagramApiTest
    {
        [Fact]
        [Description("It can extract a media from a public post without loggin in")]
        public async Task CanExtractMediaFromPublicPost()
        {
            //extract an url post from a sample tag page: https://www.instagram.com/explore/tags/art
            //example format: https://www.instagram.com/p/B-6Tug4h_cY/
            string publicUrl = null;

            Assert.NotNull(publicUrl);

            var publicPost = new Uri(publicUrl);
            using var clientHandler = new HttpClientHandler();
            using var api = new InstagramApi(clientHandler);

            var media = await api.GetMediaFromPostAsync(publicPost);

            Assert.NotEmpty(media);
        }

        [Fact]
        [Description("It cannot extract a media from a private post without loggin in")]
        public async Task CannotExtractMediaFromPrivatePostWithoutLogin()
        {
            //extract an url post from a private page
            //example format: https://www.instagram.com/p/B-4p8LtDbhKVeCTe9qS7hG8WbgdB51MjNTVNAA0/
            string privateUrl = null;

            Assert.NotNull(privateUrl);

            var privatePost = new Uri(privateUrl);
            using var clientHandler = new HttpClientHandler();
            using var api = new InstagramApi(clientHandler);

            Assert.Empty(api.UserName);

            var media = await api.GetMediaFromPostAsync(privatePost);

            Assert.Empty(media);
        }

        [Fact]
        [Description("It can extract a media from a private post if logged in")]
        public async Task CanExtractMediaFromPrivatePostWithLogin()
        {
            //set private user credential
            string userName = null;
            string password = null;

            //extract an url post from a private page that your user can see
            //example format: https://www.instagram.com/p/B-4p8LtDbhKVeCTe9qS7hG8WbgdB51MjNTVNAA0/
            string privateUrl = null;

            Assert.NotNull(userName);
            Assert.NotNull(password);
            Assert.NotNull(privateUrl);

            var privatePost = new Uri(privateUrl);
            var userCredentials = new UserCredentials(userName, password);
            using var clientHandler = new HttpClientHandler();
            using var api = new InstagramApi(clientHandler);
            var loggedIn = await api.LoginAsync(userCredentials);

            Assert.True(loggedIn);
            Assert.NotNull(api.UserName);

            var media = await api.GetMediaFromPostAsync(privatePost);

            Assert.NotEmpty(media);
        }

        [Fact]
        [Description("It can reload a user session")]
        public async Task CanReloadSession()
        {
            //set private user credential
            string userName = null;
            string password = null;

            Assert.NotNull(userName);
            Assert.NotNull(password);

            var userCredentials = new UserCredentials(userName, password);

            using var clientHandler1 = new HttpClientHandler();
            using var api1 = new InstagramApi(clientHandler1);
            var loggedIn = await api1.LoginAsync(userCredentials);

            Assert.True(api1.IsAuthenticated);

            var sessionFile = Path.GetTempFileName();
            api1.SaveSession(sessionFile);

            using var clientHandler2 = new HttpClientHandler();
            using var api2 = new InstagramApi(clientHandler2);

            Assert.False(api2.IsAuthenticated);

            api2.LoadSession(sessionFile);

            Assert.True(api2.IsAuthenticated);
        }
    }
}