using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace QuickApp.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void FirePropertyChanged(String name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private MainWindowState _State;
        public MainWindowState State
        {
            get
            {
                return _State;
            }
            set
            {
                _State = value;
                FirePropertyChanged("State");
            }
        }

        public MainWindowViewModel()
        {
            State = new MainWindowState(this);

            State.WindowWidth = 90;
            State.AppGridHeight = 100;
        }
    }
}
