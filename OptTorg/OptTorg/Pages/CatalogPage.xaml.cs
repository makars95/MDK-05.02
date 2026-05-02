using OptTorg.ViewModels;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace OptTorg.Pages
{
    public partial class CatalogPage : Page
    {
        public CatalogPage()
        {
            InitializeComponent();
            DataContext = new CatalogViewModel();
        }

        private void SortColumn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is DataGridColumnHeader header && header.Tag is string columnName)
            {
                if (DataContext is CatalogViewModel viewModel)
                {
                    viewModel.SortCommand.Execute(columnName);
                }
            }
        }
    }
}