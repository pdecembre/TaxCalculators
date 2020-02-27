using System;
using System.Linq;
using Imc.TaxCalculator.Service.AppSettingsModels;
using Imc.TaxCalculator.Service.Helpers;
using Imc.TaxCalculator.Service.RestHttpHandlers;
using Imc.TaxCalculator.Service.TaxCalculators.TaxJar;
using Imc.TaxCalculator.Service.TaxCalculators.TaxLeaf;
using Imc.TaxCalculator.Service.TaxJar;
using Imc.TaxCalculator.Service.TaxServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Imc.TaxCalculator.Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddOptions();

            IConfigurationSection config(string key) => this.Configuration.GetSection(key);

            services.Configure<TaxJarSettings>(config(SettingsConstants.TaxJarSettings));

            services.AddHttpClient<IRestHttpClientHandler, RestHttpClientHandler>(client =>
            {
                client.BaseAddress = new Uri("https://api.taxjar.com/v2");
            });
            services.AddScoped<ITaxJarHttpService, TaxJarHttpClient>();

            services.AddHttpContextAccessor();
            
            #region TaxCalculators Strategy

            // Different Tax Calculator Startegy is achieved via dependency injection
            // when a caller request a tax/tax-rates calculation through a particular route
            // the factory delegates [Func<ITaxCalculator> and Func<IRatesCalculator>] will determine
            // which provider to instantiate and execute
            // this method provides a cleaner and more direct approach 
            // than the traditional [if, switch] branching statments that the traditional strategy pattern makes use of

            services.AddScoped<Func<ITaxCalculator>>(provider => {
                var context = provider.GetService<IHttpContextAccessor>().HttpContext;
                var taxProviders = context.Request.Path.Value.Split("/".ToCharArray());
                var taxServices = provider.GetServices<ITaxCalculator>();

                return () => taxServices.FirstOrDefault(t => {
                    //TODO:  this assume that any other tax strategy implementation
                    // would follow the convension "[ProviderName]Service" i.e:  GoodTaxService, KitchenTaxService etc.... the uri would be /api/salestax/goodtax or /api/salestax/kitchentax
                    var svcName = t.GetType().Name.ToLower();
                    var providerService = taxProviders.FirstOrDefault(p => string.Equals(svcName, $"{p}service", StringComparison.OrdinalIgnoreCase));
                    return !string.IsNullOrWhiteSpace(providerService);
                });
            });

            services.AddScoped<Func<IRatesCalculator>>(provider => {

                var context = provider.GetService<IHttpContextAccessor>().HttpContext;
                var ratesProviders = context.Request.Path.Value.Split("/".ToCharArray());

                //TODO:  this assume that any other tax strategy implementation
                // would follow the convension "[ProviderName]Service" i.e:  GoodTaxService, KitchenTaxService etc.... the uri would be /api/salestax/goodtax or /api/salestax/kitchentax
                var ratesServices = provider.GetServices<IRatesCalculator>();
                return () => ratesServices.FirstOrDefault(t => {
                    var svcName = t.GetType().Name.ToLower();

                    var providerService = ratesProviders.FirstOrDefault(p => string.Equals(svcName, $"{p}service", StringComparison.OrdinalIgnoreCase));
                    return !string.IsNullOrWhiteSpace(providerService);
                });
            });
            #endregion

            services.AddTransient<ITaxCalculator, TaxJarService>();
            services.AddTransient<IRatesCalculator, TaxJarService>();
            services.AddTransient<ITaxCalculator, TaxLeafService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
