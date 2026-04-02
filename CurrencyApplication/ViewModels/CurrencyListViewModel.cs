using CommunityToolkit.Mvvm.Input;
using CurrencyApplication.Models;
using CurrencyApplication.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CurrencyApplication.ViewModels
{
    public partial class CurrencyListViewModel : BaseViewModel
    {
        private readonly ApiService _apiService = new ApiService();
        private readonly JsonStorageService _jsonStorageService = new JsonStorageService();
        private readonly SqliteStorageService _sqliteService = new SqliteStorageService();
        private readonly SettingsService _settingsService = new SettingsService();

        public ObservableCollection<Currency> Currencies { get; set; } = new ObservableCollection<Currency>();

        private Currency _selectedCurrency;
        public Currency SelectedCurrency
        {
            get => _selectedCurrency;
            set
            {
                _selectedCurrency = value;
                OnPropertyChanged(nameof(SelectedCurrency));
            }
        }

        private string _lastSessionText;
        public string LastSessionText
        {
            get => _lastSessionText;
            set
            {
                _lastSessionText = value;
                OnPropertyChanged(nameof(LastSessionText));
            }
        }

        private string _storageType = "JSON";
        public string StorageType
        {
            get => _storageType;
            set
            {
                _storageType = value;
                OnPropertyChanged(nameof(StorageType));
            }
        }

        public CurrencyListViewModel()
        {
            LoadSettingsAndData();
        }

        private async void LoadSettingsAndData()
        {
            await LoadSettingsAsync();
            await LoadLocalDataAsync();
        }

        private async Task LoadSettingsAsync()
        {
            var settings = await _settingsService.LoadAsync();

            if (settings.LastSessionTime != default)
            {
                LastSessionText = "Последний запуск: " + settings.LastSessionTime.ToString("g");
            }
            else
            {
                LastSessionText = "Первый запуск";
            }

            if (!string.IsNullOrWhiteSpace(settings.StorageType))
            {
                StorageType = settings.StorageType;
            }
            else
            {
                StorageType = "JSON";
            }
        }

        private async Task<List<Currency>> LoadDataAsync()
        {
            if (StorageType == "SQLite")
            {
                return await _sqliteService.LoadAsync();
            }

            return await _jsonStorageService.LoadAsync();
        }

        private async Task SaveDataAsync(List<Currency> currencies)
        {
            if (StorageType == "SQLite")
            {
                await _sqliteService.SaveAsync(currencies);
            }
            else
            {
                await _jsonStorageService.SaveAsync(currencies);
            }
        }

        public async Task LoadLocalDataAsync()
        {
            var localCurrencies = await LoadDataAsync();

            Currencies.Clear();

            foreach (var currency in localCurrencies.OrderBy(c => c.CharCode))
            {
                Currencies.Add(currency);
            }
        }

        [RelayCommand]
        private async Task ReloadLocalData()
        {
            await LoadLocalDataAsync();
        }

        [RelayCommand]
        private async Task LoadFromApi()
        {
            try
            {
                var apiCurrencies = await _apiService.GetCurrenciesAsync();
                var localCurrencies = await LoadDataAsync();

                var userCurrencies = localCurrencies
                    .Where(c => c.IsUserAdded)
                    .ToList();

                var mergedCurrencies = new List<Currency>();
                mergedCurrencies.AddRange(apiCurrencies);

                foreach (var userCurrency in userCurrencies)
                {
                    bool exists = mergedCurrencies.Any(c =>
                        c.Id.Equals(userCurrency.Id, StringComparison.OrdinalIgnoreCase));

                    if (!exists)
                    {
                        mergedCurrencies.Add(userCurrency);
                    }
                }

                mergedCurrencies = mergedCurrencies
                    .OrderBy(c => c.CharCode)
                    .ToList();

                Currencies.Clear();

                foreach (var currency in mergedCurrencies)
                {
                    Currencies.Add(currency);
                }

                await SaveDataAsync(mergedCurrencies);
            }
            catch
            {
                var localCurrencies = await LoadDataAsync();

                Currencies.Clear();

                foreach (var currency in localCurrencies.OrderBy(c => c.CharCode))
                {
                    Currencies.Add(currency);
                }

                MessageBox.Show("Не удалось загрузить данные из API. Показаны локальные данные.");
            }
        }

        [RelayCommand]
        private async Task DeleteCurrency()
        {
            if (SelectedCurrency == null)
            {
                MessageBox.Show("Выберите валюту для удаления.");
                return;
            }

            var result = MessageBox.Show(
                $"Удалить валюту {SelectedCurrency.Name}?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            var currencies = await LoadDataAsync();

            var itemToRemove = currencies.FirstOrDefault(c =>
                c.Id.Equals(SelectedCurrency.Id, StringComparison.OrdinalIgnoreCase));

            if (itemToRemove != null)
            {
                currencies.Remove(itemToRemove);
                await SaveDataAsync(currencies);
            }

            Currencies.Remove(SelectedCurrency);
            SelectedCurrency = null;
        }

        [RelayCommand]
        private async Task SwitchStorage()
        {
            StorageType = StorageType == "JSON" ? "SQLite" : "JSON";

            var currentData = Currencies.ToList();
            await SaveDataAsync(currentData);

            var settings = await _settingsService.LoadAsync();
            settings.StorageType = StorageType;
            await _settingsService.SaveAsync(settings);

            await LoadLocalDataAsync();

            MessageBox.Show("Текущее хранилище: " + StorageType);
        }
    }
}