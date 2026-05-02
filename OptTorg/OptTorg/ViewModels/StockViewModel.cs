namespace OptTorg.ViewModels
{
    public class StockViewModel : ViewModelBase
    {
        private string _title;
        private string _icon;

        public StockViewModel()
        {
            Title = "Складские остатки";
            Icon = "📊";
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