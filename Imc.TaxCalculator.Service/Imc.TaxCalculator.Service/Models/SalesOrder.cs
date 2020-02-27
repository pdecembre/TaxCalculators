using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imc.TaxCalculator.Service.Models
{
    public class SalesOrder
    {
        public string Country { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public double Shipping { get; set; }
        public double Amount { get; set; }
    }
}
