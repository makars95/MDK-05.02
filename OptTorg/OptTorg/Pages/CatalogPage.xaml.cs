using OptTorg.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace OptTorg.Pages
{
    public partial class CatalogPage : Page
    {
        public CatalogPage()
        {
            InitializeComponent();
            DataContext = new CatalogViewModel();
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is CatalogViewModel viewModel)
            {
                viewModel.ApplyFiltersAndSort();
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = sender as ScrollViewer;
            if (scrollViewer != null)
            {
                if (e.Delta > 0)
                    scrollViewer.LineUp();
                else
                    scrollViewer.LineDown();

                e.Handled = true;
            }
        }

        private void FilterCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (DataContext is CatalogViewModel viewModel)
            {
                var checkBox = sender as CheckBox;
                if (checkBox == null) return;

                var listBox = FindParent<ListBox>(checkBox);
                if (listBox == null) return;

                if (listBox.ItemsSource == viewModel.CategoryNames)
                {
                    string selectedItem = checkBox.Content?.ToString();

                    if (selectedItem == "Все категории" && checkBox.IsChecked == true)
                    {
                        viewModel.SelectedCategories.Clear();
                        viewModel.SelectedCategories.Add("Все категории");

                        foreach (var item in listBox.Items)
                        {
                            if (item != null && item.ToString() != "Все категории")
                            {
                                var container = listBox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                                if (container != null)
                                    container.IsSelected = false;
                            }
                        }
                    }
                    else if (selectedItem == "Все категории" && checkBox.IsChecked == false)
                    {
                        viewModel.SelectedCategories.Clear();
                    }
                    else
                    {
                        if (viewModel.SelectedCategories.Contains("Все категории"))
                        {
                            viewModel.SelectedCategories.Remove("Все категории");

                            foreach (var item in listBox.Items)
                            {
                                if (item != null && item.ToString() == "Все категории")
                                {
                                    var container = listBox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                                    if (container != null)
                                        container.IsSelected = false;
                                    break;
                                }
                            }
                        }

                        viewModel.SelectedCategories.Clear();
                        foreach (var item in listBox.SelectedItems)
                        {
                            if (item != null && item.ToString() != "Все категории")
                                viewModel.SelectedCategories.Add(item.ToString());
                        }
                    }
                }
                else if (listBox.ItemsSource == viewModel.Manufacturers)
                {
                    string selectedItem = checkBox.Content?.ToString();

                    if (selectedItem == "Все производители" && checkBox.IsChecked == true)
                    {
                        viewModel.SelectedManufacturers.Clear();
                        viewModel.SelectedManufacturers.Add("Все производители");

                        foreach (var item in listBox.Items)
                        {
                            if (item != null && item.ToString() != "Все производители")
                            {
                                var container = listBox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                                if (container != null)
                                    container.IsSelected = false;
                            }
                        }
                    }
                    else if (selectedItem == "Все производители" && checkBox.IsChecked == false)
                    {
                        viewModel.SelectedManufacturers.Clear();
                    }
                    else
                    {
                        if (viewModel.SelectedManufacturers.Contains("Все производители"))
                        {
                            viewModel.SelectedManufacturers.Remove("Все производители");

                            foreach (var item in listBox.Items)
                            {
                                if (item != null && item.ToString() == "Все производители")
                                {
                                    var container = listBox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                                    if (container != null)
                                        container.IsSelected = false;
                                    break;
                                }
                            }
                        }

                        viewModel.SelectedManufacturers.Clear();
                        foreach (var item in listBox.SelectedItems)
                        {
                            if (item != null && item.ToString() != "Все производители")
                                viewModel.SelectedManufacturers.Add(item.ToString());
                        }
                    }
                }
            }
        }

        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null) return null;

            if (parentObject is T parent)
                return parent;

            return FindParent<T>(parentObject);
        }
    }
}