using CurrencyApplication.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace CurrencyApplication.Services
{
    public class JsonStorageService
    {
        private readonly string _filePath = Path.Combine("Data", "currencies.json");

        public JsonStorageService()
        {
            var dataDirectory = "Data";

            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }
        }

        public async Task SaveAsync(List<Currency> currencies)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(currencies, options);
            await File.WriteAllTextAsync(_filePath, json);
        }

        public async Task<List<Currency>> LoadAsync()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Currency>();
            }

            var json = await File.ReadAllTextAsync(_filePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<Currency>();
            }

            var currencies = JsonSerializer.Deserialize<List<Currency>>(json);

            return currencies ?? new List<Currency>();
        }
    }
}