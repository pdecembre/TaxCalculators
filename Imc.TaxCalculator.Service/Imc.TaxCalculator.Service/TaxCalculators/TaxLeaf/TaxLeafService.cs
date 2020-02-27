using Imc.TaxCalculator.Service.Models;
using Imc.TaxCalculator.Service.TaxServices;
using System;
using System.Threading.Tasks;

namespace Imc.TaxCalculator.Service.TaxCalculators.TaxLeaf
{
    /// <summary>
    /// Dummy Tax Calculator provider to show how multiple different 
    /// tax calucation providers can be implemented 
    /// Notice that the TaxLeaf provider didn't implment the IRatesCalculator
    /// This allows the application to apply Interface Seggregation
    /// Since TaxLeaf doesn't do TaxRate calculation, than its provide shouldn't have to carry
    /// that method due to its interfac contract requirement.
    /// </summary>
    public class TaxLeafService : ITaxCalculator
    {
        public TaxLeafService()
        {}

        public async Task<double> CalculateTaxAsync(SalesOrder salesOrder)
        {
            //I hope StarBucks, chipotle dont't use this guy,
            // otherwise I should just invest that cofee/lunch money in their stock
            //on the other hand buying a new car would be sweet.

            var tax = await Task.FromResult(Convert.ToDouble(99));
            return tax;
        }


    }
}
