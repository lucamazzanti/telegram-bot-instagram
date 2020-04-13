using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
// ReSharper disable UnassignedField.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Telegram.Bot.Instagram.Tests.Mocks
{
    public class HttpClientHandlerMock : HttpClientHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;
            Response = ResponseHandler?.Invoke(request) ?? new HttpResponseMessage(HttpStatusCode.OK);
            return Task.FromResult(Response);
        }

        public Func<HttpRequestMessage, HttpResponseMessage> ResponseHandler;

        public HttpResponseMessage Response { get; set; } = new HttpResponseMessage(HttpStatusCode.OK);

        public HttpRequestMessage Request { get; private set; }
    }
}