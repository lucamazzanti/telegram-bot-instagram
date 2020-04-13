using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Telegram.Bot.Instagram.Instagram.Api.Unofficial.DTO;
using Telegram.Bot.Instagram.Instagram.Api.Unofficial.Models;
// ReSharper disable StringLiteralTypo

namespace Telegram.Bot.Instagram.Instagram.Api.Unofficial
{
    public class InstagramApi : IDisposable
    {
        private readonly HttpClientHandler _httpClientHandler;
        private readonly HttpClient _httpClient;

        private CookieContainer _cookieContainer;
        public string UserName { get; private set; }

        public InstagramApi(HttpClientHandler httpClientHandler)
        {
            _httpClientHandler = httpClientHandler ?? throw new ArgumentNullException(nameof(httpClientHandler));
            _cookieContainer = _httpClientHandler.CookieContainer ?? throw new ArgumentException("CookieContainer property mandatory", nameof(httpClientHandler));
            _httpClient = new HttpClient(_httpClientHandler) {BaseAddress = new Uri("https://www.instagram.com")};

            PrepareRequestHeaders(_httpClient.DefaultRequestHeaders);
        }

        private static void PrepareRequestHeaders(HttpRequestHeaders headers)
        {
            headers.Host = "www.instagram.com";
            headers.Referrer = new Uri("https://www.instagram.com");
            headers.ConnectionClose = false;
            headers.Add("Origin", "https://www.instagram.com");
            headers.Add("Connection", "keep-alive");
            headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.162 Safari/537.36");
            headers.Add("X-Requested-With", "XMLHttpRequest");
            headers.Add("Sec-Fetch-Site", "same-origin");
            headers.Add("Sec-Fetch-Mode", "cors");
            headers.Add("X-Instagram-AJAX", "1");
            headers.Add("X-IG-App-ID", "1");
            //headers.Add("X-CSRFToken", token);
        }

        public async Task<bool> LoginAsync(UserCredentials userCredentials)
        {
            var token = await GetTokenAsync();

            var fields = new Dictionary<string, string>
            {
                {"username", userCredentials.UserName},
                {"password", userCredentials.Password}
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "/accounts/login/ajax/")
            {
                Content = new FormUrlEncodedContent(fields)
            };

            //PrepareRequestHeaders(request.Headers);
            request.Headers.Add("X-CSRFToken", token);

            HttpResponseMessage response = await _httpClient.SendAsync(request);

            if (response.StatusCode != HttpStatusCode.OK) return false;

            UserName = userCredentials.UserName;

            return true;
        }

        private async Task<string> GetTokenAsync()
        {
            // ReSharper disable once JoinDeclarationAndInitializer
            string token;

            //first retrieval: from client cookie
            // ReSharper disable once UnusedVariable
            //HttpResponseMessage retrieveCookiesResponse = await _httpClient.GetAsync(_httpClient.BaseAddress);
            //CookieCollection cookies = _httpClientHandler.CookieContainer.GetCookies(_httpClient.BaseAddress);
            //foreach (Cookie cookie in cookies) if (cookie.Name == Constants.Csrftoken) token = cookie.Value;

            ////second retrieval: get new one from share data url
            //if (string.IsNullOrEmpty(token))
            //{
            HttpResponseMessage sharedDataResponse = await _httpClient.GetAsync("/data/shared_data/");
            if (sharedDataResponse.StatusCode != HttpStatusCode.OK) throw new InvalidOperationException("Unable to retrieve CSRF Token");
            var sharedDataRawContent = await sharedDataResponse.Content.ReadAsStringAsync();
            var sharedDataModel = JsonConvert.DeserializeObject<SharedDataContract>(sharedDataRawContent);
            token = sharedDataModel?.Config?.CSRFToken;
            //}

            if (string.IsNullOrEmpty(token)) throw new InvalidOperationException("Unable to retrieve CSRF Token");
            return token;
        }

        public void LoadSession(string sessionFile)
        {
            if (!File.Exists(sessionFile)) throw new ArgumentException("file not found", nameof(sessionFile));

            var content = File.ReadAllText(sessionFile);
            var session = JsonConvert.DeserializeObject<SessionData>(content);

            var binaryFormatter = new BinaryFormatter();
            var data = Convert.FromBase64String(session.Base64Data);
            using var stream = new MemoryStream(data);
            CookieContainer cookieContainer = binaryFormatter.Deserialize(stream) as CookieContainer;

            if (string.IsNullOrWhiteSpace(session.UserName) || cookieContainer == null) throw new Exception("error loading session");

            UserName = session.UserName;
            _cookieContainer = cookieContainer;
        }

        public void SaveSession(string sessionFile)
        {
            if (File.Exists(sessionFile)) File.Delete(sessionFile);
            if (UserName == null) throw new InvalidOperationException("no session alive");
            if (_cookieContainer == null) throw new InvalidOperationException("no session alive");

            var formatter = new BinaryFormatter();
            using var memoryStream = new MemoryStream();
            formatter.Serialize(memoryStream, _cookieContainer);

            var session = new SessionData
            {
                UserName = UserName,
                Base64Data = Convert.ToBase64String(memoryStream.ToArray())
            };
            File.WriteAllText(sessionFile, JsonConvert.SerializeObject(session));
        }

        public async Task<IEnumerable<InstagramMedia>> GetMediaFromPost(Uri uri)
        {
            throw new NotImplementedException();
        }

        public async Task<string[]> DownloadMediaFromPost(Uri uri)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
            _httpClientHandler?.Dispose();
        }
    }
}