namespace OptTorg.ViewModels
{
    public class ClientsViewModel : ViewModelBase
    {
        private string _title;
        private string _icon;

        public ClientsViewModel()
        {
            Title = "Клиенты";
            Icon = "👥";
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

        public string Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                OnPropertyChanged();
            }
        }
    }
}