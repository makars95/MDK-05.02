using System.Windows.Controls;
using OptTorg.ViewModels;

namespace OptTorg.Pages
{
    public partial class StockPage : Page
    {
        public StockPage()
        {
            InitializeComponent();
            DataContext = new StockViewModel();
        }
    }
}