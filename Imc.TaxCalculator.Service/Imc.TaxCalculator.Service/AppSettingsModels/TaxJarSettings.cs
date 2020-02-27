namespace Imc.TaxCalculator.Service.AppSettingsModels
{
    public class Auth
    {
        public string ApiKey { get; set; }
    }
    public class ApiRoutes
    {
        public string Rates { get; set; }
        public string Taxes { get; set; }
    }

    public class TaxJarSettings
    {
        public Auth Auth { get; set; }
        public ApiRoutes ApiRoutes { get; set; }
    }
}
