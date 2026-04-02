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

        public CurrencyListViewModel()
        {
            LoadLocalDataOnStartup();
            LoadLastSession();
        }

        // 🔹 Загрузка локальных данных при старте
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

        // 🔹 Загрузка last session
        private async void LoadLastSession()
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
        }

        // 🔹 Загрузка с API + merge
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

                // сортировка (красиво выглядит)
                mergedCurrencies = mergedCurrencies
                    .OrderBy(c => c.CharCode)
                    .ToList();

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

                MessageBox.Show("Ошибка API. Показаны локальные данные.");
            }
        }

        // 🔹 Удаление валюты
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

        // 🔹 Перезагрузка локальных данных (после добавления)
        [RelayCommand]
        private async Task ReloadLocalData()
        {
            await LoadLocalDataAsync();
        }
    }
}