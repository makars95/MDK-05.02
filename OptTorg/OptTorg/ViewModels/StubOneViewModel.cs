using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptTorg.ViewModels
{
    public class StubOneViewModel : ViewModelBase
    {
        public StubOneViewModel()
        {
            Title = "Страница заглушка 1";
        }

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }
    }
}