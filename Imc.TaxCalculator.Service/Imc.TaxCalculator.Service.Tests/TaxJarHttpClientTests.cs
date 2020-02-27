using AutoFixture;
using AutoFixture.AutoNSubstitute;
using FluentAssertions;
using Imc.TaxCalculator.Service.AppSettingsModels;
using Imc.TaxCalculator.Service.Helpers;
using Imc.TaxCalculator.Service.Models;
using Imc.TaxCalculator.Service.Models.TaxJar;
using Imc.TaxCalculator.Service.RestHttpHandlers;
using Imc.TaxCalculator.Service.TaxJar;
using Imc.TaxCalculator.Service.Tests.Customizations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NSubstitute;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Imc.TaxCalculator.Service.Tests
{
    [TestClass]
    public class TaxJarHttpClientTests
    {
        [TestMethod]
        public async Task TaxJar_HttpClient_CalculateTax_Should_Return_OKAsync()
        {
            var fixture = new Fixture();
            var taxResponse = fixture.Build<TaxResponse>().With(t => t.Tax, new Tax { TaxableAmount = 75, AmountToCollect = 5.64, Rate = 0.064 }).Create();
            var taxResponseMessage = JsonConvert.SerializeObject(taxResponse);

            //fixture.Customize(new CompositeCustomization(
            //       new AutoNSubstituteCustomization(),
            //       new TaxJarAppSettingsCustomizations(),
            //       new HttpClientCustomizations(HttpStatusCode.OK, taxResponseMessage)));

            fixture.Customize(new CompositeCustomization(new AutoNSubstituteCustomization(), new TaxJarAppSettingsCustomizations()));

            fixture.Customize<BindingInfo>(c => c.OmitAutoProperties());

            var httpClientHandler = fixture.Freeze<IRestHttpClientHandler>();
            IOptionsMonitor<TaxJarSettings> apiOptions = fixture.Create<IOptionsMonitor<TaxJarSettings>>();
            
            var salesOrder = fixture.Build<SalesOrder>()
                .With(a => a.Amount, 75)
                .With(b => b.Country, "US")
                .With(c => c.State, "FL")
                .With(d => d.Zip, "33461").With(e => e.Shipping, 4).Create();

            var httpRequestMessage = Substitute.For<HttpRequestMessage>();                        
            var httpResponseMessage = fixture.Build<HttpResponseMessage>()
                        .With(c => c.Content, new StringContent(taxResponseMessage))
                        .With(s => s.StatusCode, HttpStatusCode.OK)
                        .Create();

            //SUT 
            var sut = new TaxJarHttpClient(httpClientHandler, apiOptions);
            
            httpClientHandler.PostAsync(Substitute.For<HttpRequestMessage>()).ReturnsForAnyArgs(httpResponseMessage);
            
            //ACT
            var result = await sut.CalculateTaxAsync(salesOrder);

            //ASSERT
            result.Should().NotBe(null);
            Assert.IsInstanceOfType(result, typeof(double));
            result.Should().Be(taxResponse.Tax.AmountToCollect);            
        }


        [TestMethod]
        public async Task TaxJar_HttpClient_TaxRates_Should_Return_OKAsync()
        {
            var fixture = new Fixture();
            var rateResponse = fixture.Build<RateResponse>().With(r => r.Rate, new Rate { CombinedRate = "0.652" }).Create();
            var rateResponseMessage = JsonConvert.SerializeObject(rateResponse);

            //fixture.Customize(new CompositeCustomization(
            //       new AutoNSubstituteCustomization(),
            //       new TaxJarAppSettingsCustomizations(),
            //       new HttpClientCustomizations(HttpStatusCode.OK, rateResponseMessage)));

            fixture.Customize(new CompositeCustomization(new AutoNSubstituteCustomization(),new TaxJarAppSettingsCustomizations()));

            fixture.Customize<BindingInfo>(c => c.OmitAutoProperties());

            var httpClientHandler = fixture.Freeze<IRestHttpClientHandler>();

            IOptionsMonitor<TaxJarSettings> apiOptions = fixture.Create<IOptionsMonitor<TaxJarSettings>>();
            var httpResponseMessage = fixture.Build<HttpResponseMessage>()
                .With(c => c.Content, new StringContent(rateResponseMessage))
                .With(s => s.StatusCode, HttpStatusCode.OK)
                .Create();
            httpClientHandler.GetAsync(Substitute.For<HttpRequestMessage>()).ReturnsForAnyArgs(httpResponseMessage);
            
            //SUT 
            var sut = new TaxJarHttpClient(httpClientHandler, apiOptions);

            //ACT
            var result = await sut.TaxRates(Arg.Any<string>());

            //ASSERT
            result.Should().NotBe(null);
            Assert.IsInstanceOfType(result, typeof(double));
            result.Should().Be(Convert.ToDouble(rateResponse.Rate.CombinedRate));
        }

    }
}
