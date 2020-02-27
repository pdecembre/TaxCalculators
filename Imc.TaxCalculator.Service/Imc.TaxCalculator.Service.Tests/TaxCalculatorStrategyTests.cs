using AutoFixture;
using AutoFixture.AutoNSubstitute;
using FluentAssertions;
using Imc.TaxCalculator.Service.Controllers;
using Imc.TaxCalculator.Service.Exceptions;
using Imc.TaxCalculator.Service.Models;
using Imc.TaxCalculator.Service.TaxCalculators.TaxJar;
using Imc.TaxCalculator.Service.TaxCalculators.TaxLeaf;
using Imc.TaxCalculator.Service.TaxServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Threading.Tasks;

namespace Imc.TaxCalculator.Service.Tests
{
    [TestClass]
    public class TaxCalculatorStrategyTests
    {
        

        [TestMethod]
        public async Task Get_Tax_For_taxleaf_Provider_Should_Return_Instance_Of_TaxLeafService()
        {
            var fixture = new Fixture();
            fixture.Customize(new CompositeCustomization(new AutoNSubstituteCustomization()));
            fixture.Customize<BindingInfo>(c => c.OmitAutoProperties());

            var logger = fixture.Freeze<ILogger<SalesTaxController>>();
            Func<IRatesCalculator> taxRatesCalculatorDelegateFactory = Substitute.For<Func<IRatesCalculator>>();
            Func<ITaxCalculator> taxCalculatorDelegateFactory = Substitute.For<Func<ITaxCalculator>>();

            taxCalculatorDelegateFactory();
            //SUT
            var controller = new SalesTaxController(logger, taxCalculatorDelegateFactory, taxRatesCalculatorDelegateFactory);

            //ACT
            var taxRate = await controller.GetTaxRates("taxleaf", "33461");

            var webHost = Microsoft.AspNetCore.WebHost.CreateDefaultBuilder().UseStartup<Startup>().Build();
            var taxCalculatorService = webHost.Services.GetService(typeof(ITaxCalculator));

            //Assert
            taxCalculatorService.Should().BeOfType(typeof(TaxLeafService), "Because when the controller method was called the provider specified was [taxleaf] ");
        }


        [TestMethod]
        public async Task Get_Tax_Rates_For_taxjar_Provider_Should_Return_Instance_Of_TaxJarService()
        {
            var fixture = new Fixture();
            fixture.Customize(new CompositeCustomization(new AutoNSubstituteCustomization()));
            fixture.Customize<BindingInfo>(c => c.OmitAutoProperties());

            var logger = fixture.Freeze<ILogger<SalesTaxController>>();
            Func<IRatesCalculator> taxRatesCalculatorDelegateFactory = fixture.Create<Func<IRatesCalculator>>();
            Func<ITaxCalculator> taxCalculatorDelegateFactory = fixture.Create<Func<ITaxCalculator>>();

            //SUT
            var controller = new SalesTaxController(logger, taxCalculatorDelegateFactory, taxRatesCalculatorDelegateFactory);                        

            //ACT
            var taxRate = await controller.GetTaxRates("taxjar", "33461");

            taxCalculatorDelegateFactory();

            var webHost = Microsoft.AspNetCore.WebHost.CreateDefaultBuilder().UseStartup<Startup>().Build();
            var taxRatesCalculatorService = webHost.Services.GetService(typeof(IRatesCalculator));

            //Assert
            taxRatesCalculatorService.Should().BeOfType(typeof(TaxJarService), "Because when the controller method was called the provider specified was [taxjar] ");
        }    

               
    }
}
