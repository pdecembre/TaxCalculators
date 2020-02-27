using Imc.TaxCalculator.Service.AppSettingsModels;
using Imc.TaxCalculator.Service.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace Imc.TaxCalculator.Service.Helpers
{
    public static class HttpMessageConstructor
    {
        public static HttpRequestMessage ConstructTaxMessage(SalesOrder salesOrder, TaxJarSettings taxJarSettings)
        {            
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, taxJarSettings.ApiRoutes.Taxes);

            var message = new
            {
                to_country = salesOrder.Country,
                to_state = salesOrder.State,
                to_zip = salesOrder.Zip,
                shipping = salesOrder.Shipping,
                amount = salesOrder.Amount
            };

            var jsonMessage = JsonConvert.SerializeObject(message);

            var requestContent = new StringContent(jsonMessage, Encoding.UTF8, HttpConstants.ApplicationJson);
            httpRequestMessage.Content = requestContent;
            return httpRequestMessage;
        }


    }
}
