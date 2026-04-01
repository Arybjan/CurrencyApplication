using CommunityToolkit.Mvvm.Input;
using CurrencyApp.Models;
using System.Collections.ObjectModel;

namespace CurrencyApp.ViewModels;

public partial class CurrencyListViewModel : BaseViewModel
{
    public ObservableCollection<Currency> Currencies { get; set; } = new();

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
}