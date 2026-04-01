using CommunityToolkit.Mvvm.Input;
using CurrencyApplication.Models;
using CurrencyApplication.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CurrencyApplication.ViewModels
{
    public partial class CurrencyListViewModel : BaseViewModel
    {
        private readonly ApiService _apiService = new ApiService();
        private readonly JsonStorageService _jsonStorageService = new JsonStorageService();

        public ObservableCollection<Currency> Currencies { get; set; } = new ObservableCollection<Currency>();

        public CurrencyListViewModel()
        {
            LoadLocalDataOnStartup();
        }

        private async void LoadLocalDataOnStartup()
        {
            await LoadLocalDataAsync();
        }

        public async Task LoadLocalDataAsync()
        {
            var localCurrencies = await _jsonStorageService.LoadAsync();

            Currencies.Clear();

            foreach (var currency in localCurrencies)
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
        private void LoadTestData()
        {
            Currencies.Clear();

            Currencies.Add(new Currency
            {
                Id = "USD",
                CharCode = "USD",
                Name = "Доллар США",
                Nominal = 1,
                Value = 89.50m,
                Previous = 89.10m,
                IsUserAdded = false
            });

            Currencies.Add(new Currency
            {
                Id = "EUR",
                CharCode = "EUR",
                Name = "Евро",
                Nominal = 1,
                Value = 97.20m,
                Previous = 96.80m,
                IsUserAdded = false
            });
        }

        [RelayCommand]
        private async Task LoadFromApi()
        {
            try
            {
                var apiCurrencies = await _apiService.GetCurrenciesAsync();
                var localCurrencies = await _jsonStorageService.LoadAsync();

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

                Currencies.Clear();

                foreach (var currency in mergedCurrencies)
                {
                    Currencies.Add(currency);
                }

                await _jsonStorageService.SaveAsync(mergedCurrencies);
            }
            catch
            {
                var localCurrencies = await _jsonStorageService.LoadAsync();

                Currencies.Clear();

                foreach (var currency in localCurrencies)
                {
                    Currencies.Add(currency);
                }

                MessageBox.Show("Не удалось загрузить данные из API. Показаны локальные данные.");
            }
        }

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
                return;

            var currencies = await _jsonStorageService.LoadAsync();

            var itemToRemove = currencies
                .FirstOrDefault(c => c.Id == SelectedCurrency.Id);

            if (itemToRemove != null)
            {
                currencies.Remove(itemToRemove);
                await _jsonStorageService.SaveAsync(currencies);
            }

            Currencies.Remove(SelectedCurrency);
        }
    }
}