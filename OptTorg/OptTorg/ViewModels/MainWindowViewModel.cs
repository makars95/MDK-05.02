using System.Collections.ObjectModel;
using System.Linq;
using OptTorg.Models;

namespace OptTorg.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<ViewModelBase> _viewModelsCollection;
        private ViewModelBase _selectedViewModel;
        private static Polzovateli _currentUser;
        private string _title;

        public static Polzovateli CurrentUser
        {
            get => _currentUser;
            set => _currentUser = value;
        }

        public ObservableCollection<ViewModelBase> ViewModelsCollection
        {
            get => _viewModelsCollection;
            set
            {
                _viewModelsCollection = value;
                OnPropertyChanged();
            }
        }

        public ViewModelBase SelectedViewModel
        {
            get => _selectedViewModel;
            set
            {
                if (_selectedViewModel != value)
                {
                    _selectedViewModel = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public string UserFullName
        {
            get
            {
                if (CurrentUser != null)
                {
                    if (!string.IsNullOrEmpty(CurrentUser.Familiya) && !string.IsNullOrEmpty(CurrentUser.Imya))
                        return $"{CurrentUser.Familiya} {CurrentUser.Imya}";
                    else if (!string.IsNullOrEmpty(CurrentUser.Familiya))
                        return CurrentUser.Familiya;
                    else if (!string.IsNullOrEmpty(CurrentUser.Imya))
                        return CurrentUser.Imya;
                }
                return "Пользователь";
            }
        }

        public string UserPosition => "Сотрудник"; // Статичное значение, так как нет должности

        public MainWindowViewModel(Polzovateli user)
        {
            Title = "Главное окно - OptTorg";
            _currentUser = user;
            InitializeViewModels();
        }

        private void InitializeViewModels()
        {
            ViewModelsCollection = new ObservableCollection<ViewModelBase>();

            ViewModelsCollection.Add(new CatalogViewModel());
            ViewModelsCollection.Add(new ClientsViewModel());
            ViewModelsCollection.Add(new ContractsViewModel());
            ViewModelsCollection.Add(new OrdersViewModel());
            ViewModelsCollection.Add(new StockViewModel());

            SelectedViewModel = ViewModelsCollection.FirstOrDefault();
        }
    }
}