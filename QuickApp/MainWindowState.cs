using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using QuickApp.ViewModel;

namespace QuickApp
{
    public class MainWindowState : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void FirePropertyChanged(String name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private MainWindowViewModel Model { get; set; }

        public MainWindowState(MainWindowViewModel model)
        {
            Model = model;
        }

        private int _WindowWidth;
        public int WindowWidth
        {
            get
            {
                return _WindowWidth;
            }
            set
            {
                _WindowWidth = value;
                FirePropertyChanged("WindowWidth");
            }
        }

        private int _AppGridHeight;
        public int AppGridHeight
        {
            get
            {
                return _AppGridHeight;
            }
            set
            {
                _AppGridHeight = value;
                FirePropertyChanged("AppGridHeight");
            }
        }
    }
}
