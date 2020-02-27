using FluentAssertions;
using Imc.TaxCalculator.Service.AppSettingsModels;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;


namespace Imc.TaxCalculator.Service.Tests
{
    [TestClass]
    public class TaxJarOptionsTests
    {
        [TestMethod]
        public void TaxJar_AppSetting_Options_Should_Have_Values()
        {
            
            var taxJarApiOptions = new TaxJarSettings()
            {
                Auth = new Auth
                {
                    ApiKey = "@D^utyaVSSOVHU1PZFHPK<IkK212QL56663537asdLv"
                },
                ApiRoutes = new ApiRoutes
                {
                    Rates = "https://api.taxjar.com/v2/rates",
                    Taxes = "https://api.taxjar.com/v2/taxes",
                }
            };
            
            var optionMonitor = Substitute.For<IOptionsMonitor<TaxJarSettings>>();

            optionMonitor.CurrentValue.Returns(taxJarApiOptions);

            optionMonitor.Should().NotBeNull();
            optionMonitor.CurrentValue.ApiRoutes.Should().NotBeNull();
            optionMonitor.CurrentValue.Auth.Should().NotBeNull();
            optionMonitor.CurrentValue.ApiRoutes.Rates.Should().NotBeNullOrWhiteSpace();
            optionMonitor.CurrentValue.ApiRoutes.Taxes.Should().NotBeNullOrWhiteSpace();

        }

    }
}
