using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using Informer.Core;

namespace CurrencyApp
{
    public class CurrencyApplication : IInfomerApplication
    {
        private CurrencyAppControl control;
        private string path;
        private string assemblyName;
        private string friendlyName;
        private Image image;

        public CurrencyApplication()
        {
            path = string.Empty;
            assemblyName = System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Locati‌​on);
            friendlyName = "Currency application :)";
        }

        public UserControl Control
        {
            get { return control;}
            set { control = (CurrencyAppControl)value; }
        }
        public Image Image
        {
            get { return image; }
            set { image = value; }
        }

        public string AssemblyName
        {
            get { return assemblyName; }
            set { assemblyName = value; }
        }

        public string FriendlyName
        {
            get { return friendlyName; }
            set { friendlyName = value; }
        }

        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        public void RunApplication()
        {
            control = new CurrencyAppControl();
        }

        public void StopApplication()
        {
            
        }
    }
}
