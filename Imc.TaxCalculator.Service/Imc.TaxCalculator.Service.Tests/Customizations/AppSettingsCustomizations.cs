using AutoFixture;
using Imc.TaxCalculator.Service.AppSettingsModels;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Imc.TaxCalculator.Service.Tests.Customizations
{
    public class TaxJarAppSettingsCustomizations : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            var taxJarsettings = new TaxJarSettings
            {
                Auth = new Auth { ApiKey = "123456789dasdfasdfsd" },
                ApiRoutes = new ApiRoutes { Rates = "https://api.taxjar.com/v2/rates", Taxes = "https://api.taxjar.com/v2/taxes" }
            };
            var optionMonitor = fixture.Freeze<IOptionsMonitor<TaxJarSettings>>();
            optionMonitor.CurrentValue.Returns(taxJarsettings);
        }
    }
}
