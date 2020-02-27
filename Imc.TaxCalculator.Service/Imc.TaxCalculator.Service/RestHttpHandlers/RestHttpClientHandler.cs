using Imc.TaxCalculator.Service.AppSettingsModels;
using Imc.TaxCalculator.Service.Helpers;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Imc.TaxCalculator.Service.RestHttpHandlers
{


    /// <summary>
    /// Wrapper Interface around HttpClient to isolate its functionality 
    /// so that UnitTesting can be performed on the actual rest client
    /// For the purpose of this code base, only 2 of the HttpClient methods are provided
    /// ommiting other ones such as PUT and DELETE for now
    /// </summary>
    public interface IRestHttpClientHandler
    {
        Task<HttpResponseMessage> GetAsync(HttpRequestMessage httpRequestMessage);
        Task<HttpResponseMessage> PostAsync(HttpRequestMessage httpRequestMessage);
    }

    public class RestHttpClientHandler : IRestHttpClientHandler
    {

        private readonly HttpClient _httpClient;

        public RestHttpClientHandler(HttpClient httpClient, IOptionsMonitor<TaxJarSettings> apiOptions)
        {
            this._httpClient = httpClient;
            var authenticationHeaderValue = new AuthenticationHeaderValue(HttpConstants.Bearer, apiOptions.CurrentValue.Auth.ApiKey);
            this._httpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
        }

        public async Task<HttpResponseMessage> GetAsync(HttpRequestMessage httpRequestMessage)
        {
            var response = await this._httpClient.SendAsync(httpRequestMessage);
            return response;
        }

        public async Task<HttpResponseMessage> PostAsync(HttpRequestMessage httpRequestMessage)
        {
            var response = await this._httpClient.SendAsync(httpRequestMessage);
            return response;
        }
    }
}
