using CurrencyApp.ViewModels;
using System.Windows.Controls;

namespace CurrencyApp.Views;

public partial class CurrencyListPage : Page
{
    public CurrencyListPage()
    {
        InitializeComponent();
        DataContext = new CurrencyListViewModel();
    }
}