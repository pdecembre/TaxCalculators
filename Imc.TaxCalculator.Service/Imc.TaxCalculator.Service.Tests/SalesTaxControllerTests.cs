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
    public class SalesTaxControllerTests
    {
        [TestMethod]
        public void Load_Route_Should_Work()
        {

            var fixture = new Fixture();
            fixture.Customize(new CompositeCustomization(new AutoNSubstituteCustomization()));
            fixture.Customize<BindingInfo>(c => c.OmitAutoProperties());

            var logger = fixture.Freeze<ILogger<SalesTaxController>>();

            //SUT
            var controller = fixture.Create<SalesTaxController>();
            
            //Act
            var load = controller.Get().Result;

            //Assert
            Assert.IsNotNull(load);
            Assert.IsInstanceOfType(load, typeof(OkObjectResult));
            Assert.AreEqual(((OkObjectResult)load).Value, "IMC Sales Tax Service ...");

        }

        [TestMethod]
        public async Task Get_Tax_Rates_Should_WorkAsync()
        {
            var fixture = new Fixture();
            fixture.Customize(new CompositeCustomization(new AutoNSubstituteCustomization()));
            fixture.Customize<BindingInfo>(c => c.OmitAutoProperties());

            var logger = fixture.Freeze<ILogger<SalesTaxController>>();
            Func<IRatesCalculator> taxRatesCalculatorDelegateFactory = Substitute.For<Func<IRatesCalculator>>();
            Func<ITaxCalculator> taxCalculatorDelegateFactory = Substitute.For<Func<ITaxCalculator>>();

            var taxJarService = fixture.Create<TaxJarService>();

            taxRatesCalculatorDelegateFactory().Returns(t => taxJarService);

            //SUT
            var controller = new SalesTaxController(logger, taxCalculatorDelegateFactory, taxRatesCalculatorDelegateFactory);

            //ACT
            var taxRate = await controller.GetTaxRates("taxjar", "33461");

            //Assert
            taxRate.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)taxRate).StatusCode.Should().Be(200);
        }


        [TestMethod]
        public async Task Calculate_Tax_Rate_With_Invalid_Provider_Should_Return_BadRequestAsync()
        {
            var fixture = new Fixture();
            fixture.Customize(new CompositeCustomization(new AutoNSubstituteCustomization()));
            fixture.Customize<BindingInfo>(c => c.OmitAutoProperties());

            var logger = fixture.Freeze<ILogger<SalesTaxController>>();
            Func<IRatesCalculator> taxRatesCalculatorDelegateFactory = Substitute.For<Func<IRatesCalculator>>();
            Func<ITaxCalculator> taxCalculatorDelegateFactory = Substitute.For<Func<ITaxCalculator>>();

            var taxJarService = Substitute.For<ITaxCalculator>();

            taxRatesCalculatorDelegateFactory().Returns(t => null);

            //SUT
            var controller = new SalesTaxController(logger, taxCalculatorDelegateFactory, taxRatesCalculatorDelegateFactory);
                        
            //ACT
            var taxRates = await controller.GetTaxRates("Provider_Not_Doing_Tax_Rate", "33461");

            //Assert
            taxRates.Should().BeOfType<BadRequestObjectResult>();
            ((BadRequestObjectResult)taxRates).Value.Should().Be("Unable to determine TaxRates Calculator for provider: Provider_Not_Doing_Tax_Rate");

        }


        [TestMethod]
        public async Task Get_Tax_Rates_Should_Return_UnprocessableEntityAsync()
        {
            var fixture = new Fixture();
            fixture.Customize(new CompositeCustomization(new AutoNSubstituteCustomization()));
            fixture.Customize<BindingInfo>(c => c.OmitAutoProperties());

            var logger = fixture.Freeze<ILogger<SalesTaxController>>();
            Func<IRatesCalculator> taxRatesCalculatorDelegateFactory = Substitute.For<Func<IRatesCalculator>>();
            Func<ITaxCalculator> taxCalculatorDelegateFactory = Substitute.For<Func<ITaxCalculator>>();

            var taxJarService = Substitute.For<IRatesCalculator>();

            taxRatesCalculatorDelegateFactory().Returns(t => taxJarService);
                                 
            //SUT
            var controller = new SalesTaxController(logger, taxCalculatorDelegateFactory, taxRatesCalculatorDelegateFactory);
            
            taxJarService.When(async t => await t.TaxRatesAsync("33461"))
                .Do(ex => throw new TaxRateCalculationException("no tax rate for you"));

            //ACT
            var taxRate = await controller.GetTaxRates("taxjar", "33461");

            //Assert
            taxRate.Should().BeOfType<UnprocessableEntityObjectResult>();
        }


        [TestMethod]
        public async Task Calculate_Tax_Amount_Should_WorkAsync()
        {
            var fixture = new Fixture();
            fixture.Customize(new CompositeCustomization(new AutoNSubstituteCustomization()));
            fixture.Customize<BindingInfo>(c => c.OmitAutoProperties());

            var logger = fixture.Freeze<ILogger<SalesTaxController>>();
            Func<IRatesCalculator> taxRatesCalculatorDelegateFactory = Substitute.For<Func<IRatesCalculator>>();
            Func<ITaxCalculator> taxCalculatorDelegateFactory = Substitute.For<Func<ITaxCalculator>>();

            var taxJarService = fixture.Create<TaxJarService>();

            taxRatesCalculatorDelegateFactory().Returns(t => taxJarService);

            //SUT
            var controller = new SalesTaxController(logger, taxCalculatorDelegateFactory, taxRatesCalculatorDelegateFactory);
            // fixture.Create<SalesTaxController>();

            SalesOrder salesOrder = fixture.Create<SalesOrder>();
            //ACT
            var taxAmount = await controller.CalculateTaxes("taxjar", salesOrder);

            //Assert
            taxAmount.Should().BeOfType<OkObjectResult>();            
            
        }



        [TestMethod]
        public async Task Calculate_Tax_Amount_With_Invalid_Provider_Should_Return_BadRequestAsync()
        {
            var fixture = new Fixture();
            fixture.Customize(new CompositeCustomization(new AutoNSubstituteCustomization()));
            fixture.Customize<BindingInfo>(c => c.OmitAutoProperties());

            var logger = fixture.Freeze<ILogger<SalesTaxController>>();
            Func<IRatesCalculator> taxRatesCalculatorDelegateFactory = Substitute.For<Func<IRatesCalculator>>();
            Func<ITaxCalculator> taxCalculatorDelegateFactory = Substitute.For<Func<ITaxCalculator>>();

            var taxJarService = Substitute.For<ITaxCalculator>();

            taxCalculatorDelegateFactory().Returns(t => null);

            //SUT
            var controller = new SalesTaxController(logger, taxCalculatorDelegateFactory, taxRatesCalculatorDelegateFactory);

            SalesOrder salesOrder = fixture.Create<SalesOrder>();

            //ACT
            var taxAmount = await controller.CalculateTaxes("Some_Non_Existant_Provider", salesOrder);

            //Assert
            taxAmount.Should().BeOfType<BadRequestObjectResult>();
            ((BadRequestObjectResult)taxAmount).Value.Should().Be("Unable to determine Taxes Calculator for provider: Some_Non_Existant_Provider");
        }



        [TestMethod]
        public async Task Calculate_Tax_Amount_When_TaxCalulationException_Occurs_Should_Return_UnprocessableEntityAsync()
        {
            var fixture = new Fixture();
            fixture.Customize(new CompositeCustomization(new AutoNSubstituteCustomization()));
            fixture.Customize<BindingInfo>(c => c.OmitAutoProperties());

            var logger = fixture.Freeze<ILogger<SalesTaxController>>();
            Func<IRatesCalculator> taxRatesCalculatorDelegateFactory = Substitute.For<Func<IRatesCalculator>>();
            Func<ITaxCalculator> taxCalculatorDelegateFactory = Substitute.For<Func<ITaxCalculator>>();

            var taxService = Substitute.For<ITaxCalculator>();

            taxCalculatorDelegateFactory().Returns(t => taxService);

            //SUT
            var controller = new SalesTaxController(logger, taxCalculatorDelegateFactory, taxRatesCalculatorDelegateFactory);
            
            SalesOrder salesOrder = fixture.Create<SalesOrder>();
            taxService.When(async t => await t.CalculateTaxAsync(salesOrder))
                .Do(ex => throw new TaxCalculationException("today is tax free day, enjoy"));

            //ACT
            var taxAmount = await controller.CalculateTaxes("taxjar", salesOrder);

            //Assert
            taxAmount.Should().BeOfType<UnprocessableEntityObjectResult>();
            ((UnprocessableEntityObjectResult)taxAmount).Value.Should().Be("today is tax free day, enjoy");
            
        }



    }
}
