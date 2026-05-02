using System.Windows.Controls;
using OptTorg.ViewModels;

namespace OptTorg.Pages
{
    public partial class OrdersPage : Page
    {
        public OrdersPage()
        {
            InitializeComponent();
            DataContext = new OrdersViewModel();
        }
    }
}