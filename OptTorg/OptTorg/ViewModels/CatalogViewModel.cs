using Npgsql;
using OptTorg.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace OptTorg.ViewModels
{
    public class CatalogViewModel : ViewModelBase
    {
        private string _title;
        private string _icon;
        private ObservableCollection<Tovar> _tovary;
        private ObservableCollection<Tovar> _filteredTovary;
        private Tovar _selectedTovar;
        private bool _isLoading;
        private string _searchText;
        private string _connectionString = "Host=localhost;Port=5432;Database=postgres;Username=23ip1;Password=23ip1;";

        // Для сортировки и фильтрации
        private string _sortColumn;
        private System.ComponentModel.ListSortDirection _sortDirection;
        private string _selectedCategoryFilter;
        private string _selectedManufacturerFilter;

        // Команды
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SortCommand { get; }

        public CatalogViewModel()
        {
            Title = "Каталог ЛКМ";
            Icon = "🎨";
            Tovary = new ObservableCollection<Tovar>();
            FilteredTovary = new ObservableCollection<Tovar>();

            AddCommand = new RelayCommand(OpenAddForm);
            EditCommand = new RelayCommand(OpenEditForm, () => SelectedTovar != null);
            DeleteCommand = new RelayCommand(DeleteTovar, () => SelectedTovar != null);
            SortCommand = new RelayCommand<string>(ApplySort);

            LoadTovary();
        }

        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        public string Icon
        {
            get => _icon;
            set { _icon = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Tovar> Tovary
        {
            get => _tovary;
            set { _tovary = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Tovar> FilteredTovary
        {
            get => _filteredTovary;
            set { _filteredTovary = value; OnPropertyChanged(); }
        }

        public Tovar SelectedTovar
        {
            get => _selectedTovar;
            set
            {
                _selectedTovar = value;
                OnPropertyChanged();
                ((RelayCommand)EditCommand)?.RaiseCanExecuteChanged();
                ((RelayCommand)DeleteCommand)?.RaiseCanExecuteChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ApplyFiltersAndSort();
            }
        }

        public string SelectedCategoryFilter
        {
            get => _selectedCategoryFilter;
            set
            {
                _selectedCategoryFilter = value;
                OnPropertyChanged();
                ApplyFiltersAndSort();
            }
        }

        public string SelectedManufacturerFilter
        {
            get => _selectedManufacturerFilter;
            set
            {
                _selectedManufacturerFilter = value;
                OnPropertyChanged();
                ApplyFiltersAndSort();
            }
        }

        public ObservableCollection<string> Manufacturers { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> CategoryNames { get; set; } = new ObservableCollection<string>();

        private void ApplyFiltersAndSort()
        {
            var filtered = Tovary.AsEnumerable();

            // Поиск
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(t =>
                    t.Naimenovanie.ToLower().Contains(SearchText.ToLower()) ||
                    t.Proizvoditel.ToLower().Contains(SearchText.ToLower()) ||
                    t.Artikul.ToString().Contains(SearchText));
            }

            // Фильтр по категории
            if (!string.IsNullOrWhiteSpace(SelectedCategoryFilter) && SelectedCategoryFilter != "Все категории")
            {
                filtered = filtered.Where(t => t.Nazvanie_kategorii == SelectedCategoryFilter);
            }

            // Фильтр по производителю
            if (!string.IsNullOrWhiteSpace(SelectedManufacturerFilter) && SelectedManufacturerFilter != "Все производители")
            {
                filtered = filtered.Where(t => t.Proizvoditel == SelectedManufacturerFilter);
            }

            // Сортировка
            if (!string.IsNullOrEmpty(_sortColumn))
            {
                switch (_sortColumn)
                {
                    case "Artikul":
                        filtered = _sortDirection == System.ComponentModel.ListSortDirection.Ascending
                            ? filtered.OrderBy(t => t.Artikul)
                            : filtered.OrderByDescending(t => t.Artikul);
                        break;
                    case "Naimenovanie":
                        filtered = _sortDirection == System.ComponentModel.ListSortDirection.Ascending
                            ? filtered.OrderBy(t => t.Naimenovanie)
                            : filtered.OrderByDescending(t => t.Naimenovanie);
                        break;
                    default:
                        filtered = filtered.OrderBy(t => t.Naimenovanie);
                        break;
                }
            }

            FilteredTovary = new ObservableCollection<Tovar>(filtered);
        }

        private void ApplySort(string column)
        {
            if (_sortColumn == column)
            {
                _sortDirection = _sortDirection == System.ComponentModel.ListSortDirection.Ascending
                    ? System.ComponentModel.ListSortDirection.Descending
                    : System.ComponentModel.ListSortDirection.Ascending;
            }
            else
            {
                _sortColumn = column;
                _sortDirection = System.ComponentModel.ListSortDirection.Ascending;
            }
            ApplyFiltersAndSort();
        }

        public async void LoadTovary()
        {
            IsLoading = true;
            try
            {
                Tovary.Clear();
                var manufacturers = new HashSet<string>();
                var categoryNames = new HashSet<string>();

                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = @"
                        SELECT 
                            t.""Tovar_id"",
                            t.""Naimenovanie"",
                            t.""Artikul"",
                            t.""Opisanie"",
                            t.""Proizvoditel"",
                            t.""Edinica_izmereniya"",
                            t.""Kategoriya_id"",
                            COALESCE(k.""Nazvanie"", 'Без категории') AS ""Nazvanie_kategorii"",
                            t.""ImagePath""
                        FROM ""Tovari"" t
                        LEFT JOIN ""Kategoriya"" k ON t.""Kategoriya_id"" = k.""Kategoriya_id""
                        ORDER BY t.""Naimenovanie""";

                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Tovar tovar = new Tovar
                                {
                                    Tovar_id = reader.GetInt32(0),
                                    Naimenovanie = reader.IsDBNull(1) ? "Без названия" : reader.GetString(1),
                                    Artikul = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2),
                                    Opisanie = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                    Proizvoditel = reader.IsDBNull(4) ? "Не указан" : reader.GetString(4),
                                    Edinica_izmereniya = reader.IsDBNull(5) ? "шт" : reader.GetString(5),
                                    Kategoriya_id = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                                    Nazvanie_kategorii = reader.IsDBNull(7) ? "Без категории" : reader.GetString(7),
                                    ImagePath = reader.IsDBNull(8) ? null : reader.GetString(8)
                                };
                                Tovary.Add(tovar);

                                if (!string.IsNullOrEmpty(tovar.Proizvoditel) && tovar.Proizvoditel != "Не указан")
                                    manufacturers.Add(tovar.Proizvoditel);
                                if (!string.IsNullOrEmpty(tovar.Nazvanie_kategorii) && tovar.Nazvanie_kategorii != "Без категории")
                                    categoryNames.Add(tovar.Nazvanie_kategorii);
                            }
                        }
                    }
                }

                Manufacturers.Clear();
                CategoryNames.Clear();
                Manufacturers.Add("Все производители");
                CategoryNames.Add("Все категории");
                foreach (var m in manufacturers.OrderBy(x => x))
                    Manufacturers.Add(m);
                foreach (var c in categoryNames.OrderBy(x => x))
                    CategoryNames.Add(c);

                SelectedCategoryFilter = "Все категории";
                SelectedManufacturerFilter = "Все производители";

                ApplyFiltersAndSort();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void OpenAddForm()
        {
            var editWindow = new ProductEditWindow();
            if (editWindow.ShowDialog() == true)
            {
                LoadTovary();
            }
        }

        private void OpenEditForm()
        {
            if (SelectedTovar == null) return;

            var editWindow = new ProductEditWindow(SelectedTovar);
            if (editWindow.ShowDialog() == true)
            {
                LoadTovary();
            }
        }

        private async void DeleteTovar()
        {
            if (SelectedTovar == null) return;

            var result = MessageBox.Show($"Удалить товар \"{SelectedTovar.Naimenovanie}\"?\nЭто действие нельзя отменить.\nПримечание: Товар будет удален только из каталога, данные о партиях и остатках сохранятся.",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string deleteTovarSql = "DELETE FROM \"Tovari\" WHERE \"Tovar_id\" = @id";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(deleteTovarSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", SelectedTovar.Tovar_id);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                MessageBox.Show("Товар удален из каталога", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadTovary();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}