using Imc.TaxCalculator.Service.AppSettingsModels;
using Imc.TaxCalculator.Service.Exceptions;
using Imc.TaxCalculator.Service.Helpers;
using Imc.TaxCalculator.Service.Models;
using Imc.TaxCalculator.Service.Models.TaxJar;
using Imc.TaxCalculator.Service.RestHttpHandlers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Imc.TaxCalculator.Service.TaxJar
{

    public interface ITaxJarHttpService
    {
        Task<double> TaxRates(string zipCode);
        Task<double> CalculateTaxAsync(SalesOrder salesOrder);
    }

    public class TaxJarHttpClient : ITaxJarHttpService
    {
        private readonly IRestHttpClientHandler _httpClientHandler;
        private readonly IOptionsMonitor<TaxJarSettings> _apiOptions;

        public TaxJarHttpClient(IRestHttpClientHandler httpClientHandler, IOptionsMonitor<TaxJarSettings> apiOptions)
        {
            _httpClientHandler = httpClientHandler;
            _apiOptions = apiOptions;
        }

        public async Task<double> CalculateTaxAsync(SalesOrder salesOrder)
        {
            var taxJarSettings = _apiOptions.CurrentValue;
            HttpRequestMessage httpRequestMessage = HttpMessageConstructor.ConstructTaxMessage(salesOrder, taxJarSettings);

            var response = await this._httpClientHandler.PostAsync(httpRequestMessage);            
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var taxData = JsonConvert.DeserializeObject<TaxResponse>(content);
                return taxData.Tax.AmountToCollect;
            }

            var error = JsonConvert.DeserializeObject<ErrorResponse>(content);
            throw new TaxCalculationException(error?.Detail ?? "Error unable to execute provider [Taxes] calculation");
        }


        public async Task<double> TaxRates(string zipcode)
        {
            var taxJarSettings = _apiOptions.CurrentValue;
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"{taxJarSettings.ApiRoutes.Rates}/{zipcode}");

            var response = await this._httpClientHandler.GetAsync(httpRequestMessage);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var taxData = JsonConvert.DeserializeObject<RateResponse>(content);
                return Convert.ToDouble(taxData.Rate.CombinedRate);
            }

            var error = JsonConvert.DeserializeObject<ErrorResponse>(content);
            throw new TaxRateCalculationException(error?.Detail ?? "Error unable to execute provider [TaxRates] calculation");
        }
    }
}
