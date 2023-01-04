namespace ServerlessCarRent.WebSite.Services
{
    public class CurrenciesService : ICurrenciesService
    {
        private readonly static List<string> _currencies = new List<string>()
        {
            "EUR",
            "USD",
            "GBP"
        };

        public List<string> GetAll()
        {
            return _currencies.ToList();
        }
    }
}
