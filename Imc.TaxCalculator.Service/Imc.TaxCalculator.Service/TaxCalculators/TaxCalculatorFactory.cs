using Imc.TaxCalculator.Service.TaxServices;
using System;

namespace Imc.TaxCalculator.Service.TaxCalculators
{

    public class TaxCalculatorFactory
    {
        public Func<ITaxCalculator> GetTaxCalculator;

        public Func<IRatesCalculator> GetTaxRatesCalculator;
    }
    
}
