using CurrencyApplication.ViewModels;
using CurrencyApplication.Views;
using System.Windows;

namespace CurrencyApplication
{
    public partial class MainWindow : Window
    {
        private readonly CurrencyListPage _currencyListPage;

        public MainWindow()
        {
            InitializeComponent();

            _currencyListPage = new CurrencyListPage();
            MainFrame.Navigate(_currencyListPage);
        }

        private async void CurrenciesButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currencyListPage.DataContext is CurrencyListViewModel vm)
            {
                await vm.LoadLocalDataAsync();
            }

            MainFrame.Navigate(_currencyListPage);
        }

        private void AddCurrencyButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AddCurrencyPage());
        }
    }
}