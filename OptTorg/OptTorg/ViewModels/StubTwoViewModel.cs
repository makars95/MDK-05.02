using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptTorg.ViewModels
{
    public class StubTwoViewModel : ViewModelBase
    {
        public StubTwoViewModel()
        {
            Title = "Страница заглушка 2";
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
