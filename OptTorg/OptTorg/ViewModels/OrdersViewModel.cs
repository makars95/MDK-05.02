namespace OptTorg.ViewModels
{
    public class OrdersViewModel : ViewModelBase
    {
        private string _title;
        private string _icon;

        public OrdersViewModel()
        {
            Title = "Заказы клиентов";
            Icon = "📋";
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