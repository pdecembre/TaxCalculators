using AutoFixture;
using Imc.TaxCalculator.Service.Tests.HttpHandlers;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Imc.TaxCalculator.Service.Tests.Customizations
{
    /// <summary>
    /// This AutoFixture customization allows the mocking of http requests 
    /// using hte MockHttpMessageHandler 
    /// this way we can determine what status code and response message 
    /// to generate for the unit test method making use of that HttpClientCustomizations
    /// </summary>
    public class HttpClientCustomizations : ICustomization
    {
        private readonly HttpStatusCode _httpStatusCode;
        private readonly string _responseContent;

        public HttpClientCustomizations(HttpStatusCode httpStatusCode, string responseContent)
        {
            _httpStatusCode = httpStatusCode;
            _responseContent = responseContent;
        }

        public void Customize(IFixture fixture)
        {
            var httpClient = new HttpClient(new MockHttpMessageHandler(async (request, cancellationToken) =>
            {
                var responseMessage = new HttpResponseMessage(_httpStatusCode)
                {
                    Content = new StringContent(_responseContent)
                };

                return await Task.FromResult(responseMessage);
            }));

            //fixture.Customize<HttpClient>(c => c.FromFactory<HttpClient>(h => httpClient));

            fixture.Customize<HttpClient>(c => c.FromSeed(h => httpClient));

        }
    }
}
