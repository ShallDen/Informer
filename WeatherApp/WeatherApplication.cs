using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Informer.Core;
using System.Reflection;
using WeatherApp.Properties;
using Informer.Utils;

namespace WeatherApp
{
    public class WeatherApplication : IInfomerApplication
    {
        private WeatherAppControl mControl;
        private string mPath;
        private string mAssemblyName;
        private string mFriendlyName;
        private byte[] mImage;

        public WeatherApplication()
        {
            mPath = Assembly.GetExecutingAssembly().Locati‌​on;
            mAssemblyName = System.IO.Path.GetFileNameWithoutExtension(mPath);
            mFriendlyName = "Weather application :)";
            mImage = ImageHelper.ImageToByteArray(Resources.weather_icon_png_16);
        }

        public UserControl Control
        {
            get { return mControl; }
            set { mControl = (WeatherAppControl)value; }
        }

        public byte[] Image
        {
            get { return mImage; }
            set { mImage = value; }
        }

        public string AssemblyName
        {
            get { return mAssemblyName; }
            set { mAssemblyName = value; }
        }

        public string FriendlyName
        {
            get { return mFriendlyName; }
            set { mFriendlyName = value; }
        }

        public string Path
        {
            get { return mPath; }
            set { mPath = value; }
        }

        public void RunApplication()
        {
            mControl = new WeatherAppControl();
        }

        public void StopApplication()
        {
            if (mControl != null)
            {
                mControl.StopApplicationControl();
            }
        }
    }
}
