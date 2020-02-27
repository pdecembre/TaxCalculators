using Imc.TaxCalculator.Service.Models;
using System.Threading.Tasks;

namespace Imc.TaxCalculator.Service.TaxServices
{
    public interface IRatesCalculator
    {
        Task<double> TaxRatesAsync(string zipCode);
    }

    public interface ITaxCalculator
    {
        Task<double> CalculateTaxAsync(SalesOrder salesOrder);
    }


}
