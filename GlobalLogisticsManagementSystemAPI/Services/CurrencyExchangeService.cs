using System.Text.Json;

namespace GlobalLogisticsManagementSystemAPI.Services
{
    public interface ICurrencyExchangeService
    {
        Task<decimal> GetUsdToZarRateAsync();
    }

    public class CurrencyExchangeService : ICurrencyExchangeService
    {
        private readonly HttpClient _httpClient;

        public CurrencyExchangeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetUsdToZarRateAsync()
        {
            var response = await _httpClient.GetAsync("https://open.er-api.com/v6/latest/USD");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            var rate = doc.RootElement.GetProperty("rates").GetProperty("ZAR").GetDecimal();

            return rate;
        }
    }
}
