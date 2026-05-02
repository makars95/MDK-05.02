using System.Windows.Controls;
using OptTorg.ViewModels;

namespace OptTorg.Pages
{
    public partial class ClientsPage : Page
    {
        public ClientsPage()
        {
            InitializeComponent();
            DataContext = new ClientsViewModel();
        }
    }
}