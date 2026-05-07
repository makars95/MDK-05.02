using Npgsql;
using OptTorg.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

        private string _sortColumn;
        private System.ComponentModel.ListSortDirection _sortDirection;

        private ObservableCollection<string> _selectedCategories;
        private ObservableCollection<string> _selectedManufacturers;
        private int _selectedSort;
        private int _currentCountElements;
        private int _countAllElements;
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

            SelectedCategories = new ObservableCollection<string>();
            SelectedManufacturers = new ObservableCollection<string>();

            SelectedCategories.CollectionChanged += OnFilterCollectionChanged;
            SelectedManufacturers.CollectionChanged += OnFilterCollectionChanged;

            AddCommand = new RelayCommand(OpenAddForm);
            EditCommand = new RelayCommand(OpenEditForm, CanExecuteEditDelete);
            DeleteCommand = new RelayCommand(DeleteTovar, CanExecuteEditDelete);
            SortCommand = new RelayCommand<string>(ApplySort);

            LoadTovary();
        }

        private bool CanExecuteEditDelete()
        {
            return SelectedTovar != null;
        }

        private void OnFilterCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ApplyFiltersAndSort();
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged(); }
        }

        public string Icon
        {
            get { return _icon; }
            set { _icon = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Tovar> Tovary
        {
            get { return _tovary; }
            set { _tovary = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Tovar> FilteredTovary
        {
            get { return _filteredTovary; }
            set { _filteredTovary = value; OnPropertyChanged(); }
        }

        public Tovar SelectedTovar
        {
            get { return _selectedTovar; }
            set
            {
                _selectedTovar = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }
        public int SelectedSort
        {
            get { return _selectedSort; }
            set
            {
                _selectedSort = value;
                OnPropertyChanged();
                ApplyFiltersAndSort();
            }
        }
        public int CurrentCountElements
        {
            get { return _currentCountElements; }
            set { _currentCountElements = value; OnPropertyChanged(); }
        }

        public int CountAllElements
        {
            get { return _countAllElements; }
            set { _countAllElements = value; OnPropertyChanged(); }
        }
        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ApplyFiltersAndSort();
            }
        }

        public ObservableCollection<string> SelectedCategories
        {
            get { return _selectedCategories; }
            set { _selectedCategories = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> SelectedManufacturers
        {
            get { return _selectedManufacturers; }
            set { _selectedManufacturers = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> Manufacturers { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> CategoryNames { get; set; } = new ObservableCollection<string>();

        public void ApplyFiltersAndSort()
        {
            IEnumerable<Tovar> filtered = Tovary.AsEnumerable();

            CountAllElements = Tovary.Count;

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string searchLower = SearchText.ToLower();
                filtered = filtered.Where(t =>
                    (t.Naimenovanie != null && t.Naimenovanie.ToLower().Contains(searchLower)) ||
                    (t.Proizvoditel != null && t.Proizvoditel.ToLower().Contains(searchLower)) ||
                    t.Artikul.ToString().Contains(SearchText));
            }

            if (SelectedCategories != null && SelectedCategories.Count > 0)
            {
                bool hasAllCategories = SelectedCategories.Contains("Все категории");

                if (!hasAllCategories)
                {
                    filtered = filtered.Where(t =>
                        SelectedCategories.Contains(t.Nazvanie_kategorii ?? "Без категории"));
                }
            }

            if (SelectedManufacturers != null && SelectedManufacturers.Count > 0)
            {
                bool hasAllManufacturers = SelectedManufacturers.Contains("Все производители");

                if (!hasAllManufacturers)
                {
                    filtered = filtered.Where(t =>
                        SelectedManufacturers.Contains(t.Proizvoditel ?? "Не указан"));
                }
            }

            switch (SelectedSort)
            {
                case 0:
                    filtered = filtered.OrderBy(t => t.Naimenovanie);
                    break;
                case 1:
                    filtered = filtered.OrderByDescending(t => t.Naimenovanie);
                    break;
                case 2:
                    filtered = filtered.OrderBy(t => t.Artikul);
                    break;
                case 3:
                    filtered = filtered.OrderByDescending(t => t.Artikul);
                    break;
                default:
                    filtered = filtered.OrderBy(t => t.Naimenovanie);
                    break;
            }

            var filteredList = filtered.ToList();
            CurrentCountElements = filteredList.Count;
            FilteredTovary = new ObservableCollection<Tovar>(filteredList);
        }

        private void ApplySort(string column)
        {
            if (_sortColumn == column)
            {
                if (_sortDirection == System.ComponentModel.ListSortDirection.Ascending)
                    _sortDirection = System.ComponentModel.ListSortDirection.Descending;
                else
                    _sortDirection = System.ComponentModel.ListSortDirection.Ascending;
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
                HashSet<string> manufacturers = new HashSet<string>();
                HashSet<string> categoryNames = new HashSet<string>();

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

                                if (!string.IsNullOrEmpty(tovar.Nazvanie_kategorii))
                                    categoryNames.Add(tovar.Nazvanie_kategorii);
                                if (!string.IsNullOrEmpty(tovar.Proizvoditel))
                                    manufacturers.Add(tovar.Proizvoditel);
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

                ApplyFiltersAndSort();
            }
            catch (Exception ex)
            {
                AppMessages.ShowError($"Ошибка загрузки данных: {ex.Message}", "Ошибка");
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

            var result = AppMessages.ShowQuestion(
                $"Удалить товар \"{SelectedTovar.Naimenovanie}\"?\nЭто действие нельзя отменить. Подтверждение удаления");

            if (result != CustomMessageBox.MessageBoxResult.Yes)
                return;

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

                AppMessages.ShowInfo("Товар удален из каталога", "Успех");
                LoadTovary();
            }
            catch (Exception ex)
            {
                AppMessages.ShowError($"Ошибка удаления: {ex.Message}", "Ошибка");
            }
        }
    }
}