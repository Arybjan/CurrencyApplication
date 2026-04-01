using CommunityToolkit.Mvvm.Input;
using CurrencyApplication.Models;
using CurrencyApplication.Services;
using System.Threading.Tasks;
using System.Windows;

namespace CurrencyApplication.ViewModels
{
    public partial class AddCurrencyViewModel : BaseViewModel
    {
        private readonly JsonStorageService _jsonStorageService = new JsonStorageService();

        private string _id = string.Empty;
        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        private string _charCode = string.Empty;
        public string CharCode
        {
            get { return _charCode; }
            set
            {
                _charCode = value;
                OnPropertyChanged(nameof(CharCode));
            }
        }

        private string _name = string.Empty;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private int _nominal = 1;
        public int Nominal
        {
            get { return _nominal; }
            set
            {
                _nominal = value;
                OnPropertyChanged(nameof(Nominal));
            }
        }

        private decimal _value;
        public decimal Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        private decimal _previous;
        public decimal Previous
        {
            get { return _previous; }
            set
            {
                _previous = value;
                OnPropertyChanged(nameof(Previous));
            }
        }

        [RelayCommand]
        private async Task SaveCurrency()
        {
            if (string.IsNullOrWhiteSpace(Id) ||
                string.IsNullOrWhiteSpace(CharCode) ||
                string.IsNullOrWhiteSpace(Name))
            {
                MessageBox.Show("Заполните обязательные поля: Id, CharCode, Name.");
                return;
            }

            var currencies = await _jsonStorageService.LoadAsync();

            var existing = currencies.Find(c => c.Id.ToUpper() == Id.ToUpper());

            if (existing != null)
            {
                MessageBox.Show("Валюта с таким Id уже существует.");
                return;
            }

            var newCurrency = new Currency
            {
                Id = Id.ToUpper(),
                CharCode = CharCode.ToUpper(),
                Name = Name,
                Nominal = Nominal,
                Value = Value,
                Previous = Previous,
                IsUserAdded = true
            };

            currencies.Add(newCurrency);

            await _jsonStorageService.SaveAsync(currencies);

            MessageBox.Show("Пользовательская валюта успешно сохранена.");

            ClearFields();
        }

        private void ClearFields()
        {
            Id = string.Empty;
            CharCode = string.Empty;
            Name = string.Empty;
            Nominal = 1;
            Value = 0;
            Previous = 0;
        }
    }
}