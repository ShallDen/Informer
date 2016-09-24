using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Informer.Core;
using System.Reflection;

namespace WeatherApp
{
    public class WeatherApplication : IInfomerApplication
    {
        private WeatherAppControl control;
        private string path;
        private string assemblyName;
        private string friendlyName;
        private Image image;

        public WeatherApplication()
        {
            path = string.Empty;
            assemblyName = System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Locati‌​on);
            friendlyName = "Weather application :)";
        }

        public UserControl Control
        {
            get { return control; }
            set { control = (WeatherAppControl)value; }
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
            control = new WeatherAppControl();
        }

        public void StopApplication()
        {
            if (control != null)
            {
                control.StopApplicationControl();
            }
        }
    }
}
