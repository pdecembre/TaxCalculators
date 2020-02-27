using System;

namespace Imc.TaxCalculator.Service.Exceptions
{
    public class TaxCalculationException : Exception
    {
        public TaxCalculationException(string message) : base(message)
        {}

        public TaxCalculationException(string message, Exception innerException) : base(message, innerException)
        {}
    }


    public class TaxRateCalculationException : Exception
    {
        public TaxRateCalculationException(string message) : base(message)
        {}

        public TaxRateCalculationException(string message, Exception innerException) : base(message, innerException)
        {}
    }

}
