using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Telegram.Bot.Instagram.Instagram.Api.Unofficial;
using Telegram.Bot.Instagram.Instagram.Api.Unofficial.Models;
using Telegram.Bot.Instagram.Tests.Mocks;
using Xunit;
// ReSharper disable StringLiteralTypo
// ReSharper disable ConvertToUsingDeclaration

namespace Telegram.Bot.Instagram.Tests.Instagram.Api.Unofficial
{
    public class InstagramApiTest
    {
        [Fact]
        public async Task LoginAsync_NotNullUserCredentials_ExpectedRequestFormat()
        {
            var clientHandler = new InstagramClientHandlerMock();
            using var api = new InstagramApi(clientHandler);
            var userCredentials = new UserCredentials("johndoe", "123");
            var isLogged = await api.LoginAsync(userCredentials);

            Assert.True(isLogged);

            HttpRequestMessage request = clientHandler.LastRequest;
            var headers = request.Headers.ToDictionary(t => t.Key, t => t.Value.ToArray());
            var content = request.Content.ReadAsStringAsync().Result;

            /*
            POST https://www.instagram.com/accounts/login/ajax/ HTTP/1.1
            Host: www.instagram.com
            Connection: keep-alive
            Content-Length: 295
            X-IG-WWW-Claim: hmac.AR1Z9s53CtGTOWRd_KWw-fadYaRA89Ai9V2TlSUEtbg91Uww
            X-Instagram-AJAX: 3aed6acc7f7e
            Content-Type: application/x-www-form-urlencoded
            Accept: * /*
            Sec-Fetch-Dest: empty
            X-Requested-With: XMLHttpRequest
            User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.162 Safari/537.36
            X-CSRFToken: PtOw40RiRcN5bBVEoZUttkEbLetmO4N6
            X-IG-App-ID: 936619743392333
            Origin: https://www.instagram.com
            Sec-Fetch-Site: same-origin
            Sec-Fetch-Mode: cors
            Referer: https://www.instagram.com/
            Accept-Encoding: gzip, deflate, br
            Accept-Language: en,it;q=0.9,en-US;q=0.8,de;q=0.7,es;q=0.6,ru;q=0.5,tr;q=0.4,zh-CN;q=0.3,zh;q=0.2,pt;q=0.1,da;q=0.1
            Cookie: csrftoken=PtOw40RiRcN5bBVEoZUttkEbLetmO444;"

            username=johndoe&password=123            
            */

            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("https://www.instagram.com/accounts/login/ajax/", request.RequestUri.AbsoluteUri);
            Assert.Equal("application/x-www-form-urlencoded", request.Content.Headers.ContentType.MediaType);
            Assert.Equal("keep-alive", request.Headers.Connection.ToString());
            Assert.Equal("https://www.instagram.com", headers["Origin"][0]);
            Assert.Equal("https://www.instagram.com/", request.Headers.Referrer.AbsoluteUri);
            Assert.Equal("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.162 Safari/537.36", request.Headers.UserAgent.ToString());
            Assert.Equal("XMLHttpRequest", headers["X-Requested-With"][0]);
            Assert.Equal("same-origin", headers["Sec-Fetch-Site"][0]);
            Assert.Equal("cors", headers["Sec-Fetch-Mode"][0]);
            Assert.Equal("1", headers["X-Instagram-AJAX"][0]);
            Assert.Equal("1", headers["X-IG-App-ID"][0]);
            Assert.NotNull(headers["X-CSRFToken"][0]);
            Assert.Equal("username=johndoe&password=123", content);
        }

        [Fact]
        public async Task LoginAsync_CorrectUserCredentials_ReturnsTrueAndSetUserName()
        {
            var clientHandler = new InstagramClientHandlerMock();
            using var api = new InstagramApi(clientHandler);
            Assert.Null(api.UserName);

            var userCredentials = new UserCredentials("johndoe", "123");
            var isLogged = await api.LoginAsync(userCredentials);

            Assert.True(isLogged);
            Assert.Equal(userCredentials.UserName, api.UserName);
        }

        [Fact]
        public async Task LoginAsync_WrongUserCredentials_ReturnsFalse()
        {
            var clientHandler = new InstagramClientHandlerMock();
            clientHandler.SetLoginResponse(new HttpResponseMessage(HttpStatusCode.Unauthorized));
            using var api = new InstagramApi(clientHandler);
            Assert.Null(api.UserName);

            var userCredentials = new UserCredentials("johndoe", "000");
            var isLogged = await api.LoginAsync(userCredentials);

            Assert.Null(api.UserName);
            Assert.False(isLogged);
        }

        [Fact]
        public async Task SaveSession_UserLoggedIn_ItStoreCredentialsInFile()
        {
            var sessionFile = Path.GetTempFileName();
            var clientHandler = new InstagramClientHandlerMock();
            using var api = new InstagramApi(clientHandler);
            var userCredentials = new UserCredentials("johndoe", "000");
            var isLogged = await api.LoginAsync(userCredentials);

            Assert.NotNull(api.UserName);
            Assert.True(isLogged);

            api.SaveSession(sessionFile);

            Assert.True(File.Exists(sessionFile));
            Assert.NotEqual(0, new FileInfo(sessionFile).Length);
        }

        [Fact]
        public void LoadSession_NotNullSessionFile_ItSetsUserCredentials()
        {
            const string sessionFileContent =
                "{\"UserName\":\"johndoe\",\"Base64Data\":\"AAEAAAD/////AQAAAAAAAAAMAgAAAElTeXN0ZW0sIFZlcnNpb249NC4wLjAuMCwgQ3VsdHVyZT1uZX" +
                "V0cmFsLCBQdWJsaWNLZXlUb2tlbj1iNzdhNWM1NjE5MzRlMDg5BQEAAAAaU3lzdGVtLk5ldC5Db29raWVDb250YWluZXIGAAAADW1fZG9tYWluVGFibGUPbV9" +
                "tYXhDb29raWVTaXplDG1fbWF4Q29va2llcxVtX21heENvb2tpZXNQZXJEb21haW4HbV9jb3VudA5tX2ZxZG5NeURvbWFpbgMAAAAAARxTeXN0ZW0uQ29sbGVj" +
                "dGlvbnMuSGFzaHRhYmxlCAgICAIAAAAJAwAAAAAQAAAsAQAAFAAAAAAAAAAGBAAAAAouQVJYLmxvY2FsBAMAAAAcU3lzdGVtLkNvbGxlY3Rpb25zLkhhc2h0Y" +
                "WJsZQcAAAAKTG9hZEZhY3RvcgdWZXJzaW9uCENvbXBhcmVyEEhhc2hDb2RlUHJvdmlkZXIISGFzaFNpemUES2V5cwZWYWx1ZXMAAAMDAAUFCwgcU3lzdGVtLk" +
                "NvbGxlY3Rpb25zLklDb21wYXJlciRTeXN0ZW0uQ29sbGVjdGlvbnMuSUhhc2hDb2RlUHJvdmlkZXII7FE4PwAAAAAKCgMAAAAJBQAAAAkGAAAAEAUAAAAAAAA" +
                "AEAYAAAAAAAAACw==\"}";

            var sessionFile = Path.GetTempFileName();
            File.WriteAllText(sessionFile, sessionFileContent);
            var clientHandler = new InstagramClientHandlerMock();
            using var api = new InstagramApi(clientHandler);

            Assert.Null(api.UserName);

            api.LoadSession(sessionFile);

            Assert.Equal("johndoe", api.UserName);
        }
    }
}