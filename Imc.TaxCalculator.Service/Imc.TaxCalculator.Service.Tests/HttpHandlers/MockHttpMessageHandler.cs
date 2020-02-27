using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Imc.TaxCalculator.Service.Tests.HttpHandlers
{
    //Since the HttpMessageHandler is the one that actually does all the work
    //in processing and sending http messages,
    //this mock HttpMessage handler can be used to test sending messages using HttpClient

    public interface IMockHttpMessageHandler
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);
    }

    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _sendAsync;

        public MockHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> sendAsync)
        {
            _sendAsync = sendAsync;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await _sendAsync(request, cancellationToken);
        }
    }
}
