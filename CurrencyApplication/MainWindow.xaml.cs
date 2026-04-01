using CurrencyApp.Views;
using System.Windows;

namespace CurrencyApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainFrame.Navigate(new CurrencyListPage());
    }

    private void CurrenciesButton_Click(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new CurrencyListPage());
    }

    private void AddCurrencyButton_Click(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new AddCurrencyPage());
    }
}