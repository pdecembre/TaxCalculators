using Imc.TaxCalculator.Service.Models;
using Imc.TaxCalculator.Service.TaxJar;
using Imc.TaxCalculator.Service.TaxServices;
using System.Threading.Tasks;

namespace Imc.TaxCalculator.Service.TaxCalculators.TaxJar
{
    public class TaxJarService : IRatesCalculator, ITaxCalculator
    {
        private readonly ITaxJarHttpService _httpService;

        public TaxJarService(ITaxJarHttpService httpService)
        {
            this._httpService = httpService;
        }

        public async Task<double> CalculateTaxAsync(SalesOrder salesOrder)
        {
            return await this._httpService.CalculateTaxAsync(salesOrder);
        }

        public async Task<double> TaxRatesAsync(string zipCode)
        {
            return await this._httpService.TaxRates(zipCode);
        }
    }
}
