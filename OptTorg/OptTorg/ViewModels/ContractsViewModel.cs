namespace OptTorg.ViewModels
{
    public class ContractsViewModel : ViewModelBase
    {
        private string _title;
        private string _icon;

        public ContractsViewModel()
        {
            Title = "Договоры";
            Icon = "📄";
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