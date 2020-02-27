using System;
using System.Threading.Tasks;
using Imc.TaxCalculator.Service.Exceptions;
using Imc.TaxCalculator.Service.Models;
using Imc.TaxCalculator.Service.TaxServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Imc.TaxCalculator.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesTaxController : ControllerBase
    {

        private readonly ILogger<SalesTaxController> _logger;
        private readonly Func<ITaxCalculator> taxCalculatorDelegateFactory;
        private readonly Func<IRatesCalculator> taxRatesCalculatorDelegateFactory;
        
        public SalesTaxController(ILogger<SalesTaxController> logger, Func<ITaxCalculator> taxCalculatorDelegateFactory, Func<IRatesCalculator> taxRatesCalculatorDelegateFactory)
        {
            _logger = logger;
            this.taxCalculatorDelegateFactory = taxCalculatorDelegateFactory;
            this.taxRatesCalculatorDelegateFactory = taxRatesCalculatorDelegateFactory;
        }

        [HttpGet]
        [Route("load")]
        public ActionResult<string> Get()
        {
            var taxSvc = "IMC Sales Tax Service ...";
            return Ok(taxSvc);

        }

        [HttpGet]
        [Route("rates/{provider}/{zipcode}")]
        public async Task<ActionResult> GetTaxRates(string provider, string zipCode)
        {
            _logger.Log(LogLevel.Information, "Getting Tax Rate Completed");

            try
            {
                var calculator = this.taxRatesCalculatorDelegateFactory();
               
                if (calculator == null)
                {
                    return BadRequest($"Unable to determine TaxRates Calculator for provider: {provider}");
                }
                var taxRates = await calculator.TaxRatesAsync(zipCode);
                return Ok(taxRates);
            }
            catch (TaxRateCalculationException rxe)
            {
                _logger.LogError(rxe.Message);
                return UnprocessableEntity(rxe.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(ex.Message);
            }
        }


        [HttpPost]
        [Route("taxes/{provider}")]
        public async Task<ActionResult> CalculateTaxes(string provider, [FromBody]SalesOrder salesOrder)
        {
            _logger.Log(LogLevel.Information, "Calculate Tax Rate Completed");

            try
            {                
                var calculator = this.taxCalculatorDelegateFactory();
                if (calculator == null)
                {
                    return BadRequest($"Unable to determine Taxes Calculator for provider: {provider}");
                }
                var tax = await calculator.CalculateTaxAsync(salesOrder);
                return Ok(tax);
            }
            catch(TaxCalculationException txe)
            {
                _logger.LogError(txe.Message);
                return UnprocessableEntity(txe.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(ex.Message);
            }
        }
    }
}
