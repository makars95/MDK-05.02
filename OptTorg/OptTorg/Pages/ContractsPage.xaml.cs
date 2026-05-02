using System.Windows.Controls;
using OptTorg.ViewModels;

namespace OptTorg.Pages
{
    public partial class ContractsPage : Page
    {
        public ContractsPage()
        {
            InitializeComponent();
            DataContext = new ContractsViewModel();
        }
    }
}