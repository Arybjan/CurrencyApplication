using CurrencyApplication.ViewModels;
using System.Windows.Controls;

namespace CurrencyApplication.Views
{
    public partial class CurrencyListPage : Page
    {
        public CurrencyListPage()
        {
            InitializeComponent();
            DataContext = new CurrencyListViewModel();
        }
    }
}