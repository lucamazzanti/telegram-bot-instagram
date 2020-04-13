using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Instagram.Instagram.Api.Unofficial.DTO;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Telegram.Bot.Instagram.Tests.Mocks
{
    public class InstagramClientHandlerMock : HttpClientHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;

            HttpResponseMessage responseMessage;

            if (request.RequestUri == new Uri("https://www.instagram.com/data/shared_data/"))
            {
                responseMessage = GetSharedDataResponseMessage();
            }
            else if (request.RequestUri == new Uri("https://www.instagram.com/accounts/login/ajax/"))
            {
                responseMessage = _loginResponseMessage;
            }
            else
            {
                throw new NotSupportedException("request not configured in mock");
            }

            LastResponse = responseMessage;

            return Task.FromResult(responseMessage);
        }

        private HttpResponseMessage GetSharedDataResponseMessage()
        {
            var sharedDataModel = JsonConvert.SerializeObject(new SharedDataContract
            {
                Config = new ConfigType
                {
                    CSRFToken = "1"
                }
            });

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(sharedDataModel)
            };
        }

        private HttpResponseMessage _loginResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);

        public void SetLoginResponse(HttpResponseMessage responseMessage)
        {
            _loginResponseMessage = responseMessage ?? throw new ArgumentNullException(nameof(responseMessage));
        }

        public HttpRequestMessage LastRequest { get; private set; }

        public HttpResponseMessage LastResponse { get; set; }
    }
}