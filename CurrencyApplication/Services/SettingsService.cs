using CurrencyApplication.Models;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace CurrencyApplication.Services
{
    public class SettingsService
    {
        private readonly string _filePath;

        public SettingsService()
        {
            var dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
            }

            _filePath = Path.Combine(dataDir, "settings.json");
        }

        public async Task SaveAsync(AppSettings settings)
        {
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(_filePath, json);
        }

        public async Task<AppSettings> LoadAsync()
        {
            if (!File.Exists(_filePath))
            {
                return new AppSettings();
            }

            var json = await File.ReadAllTextAsync(_filePath);

            return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }
    }
}