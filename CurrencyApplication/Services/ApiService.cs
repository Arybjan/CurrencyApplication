using CurrencyApplication.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CurrencyApplication.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<List<Currency>> GetCurrenciesAsync()
        {
            var url = "https://www.cbr-xml-daily.ru/daily_json.js";

            var json = await _httpClient.GetStringAsync(url);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var response = JsonSerializer.Deserialize<CbrResponse>(json, options);

            var result = new List<Currency>();

            foreach (var item in response.Valute)
            {
                result.Add(new Currency
                {
                    Id = item.Key,
                    CharCode = item.Value.CharCode,
                    Nominal = item.Value.Nominal,
                    Name = item.Value.Name,
                    Value = item.Value.Value,
                    Previous = item.Value.Previous,
                    IsUserAdded = false
                });
            }

            return result;
        }
    }
}