using CurrencyApplication.ViewModels;
using System.Windows.Controls;

namespace CurrencyApplication.Views
{
    public partial class AddCurrencyPage : Page
    {
        public AddCurrencyPage()
        {
            InitializeComponent();
            DataContext = new AddCurrencyViewModel();
        }
    }
}