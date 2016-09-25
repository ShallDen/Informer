using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using Informer.Core;
using CurrencyApp.Properties;
using Informer.Utils;

namespace CurrencyApp
{
    public class CurrencyApplication : IInfomerApplication
    {
        private CurrencyAppControl mControl;
        private string mPath;
        private string mAssemblyName;
        private string mFriendlyName;
        private byte[] mImage;

        public CurrencyApplication()
        {
            mPath = Assembly.GetExecutingAssembly().Locati‌​on;
            mAssemblyName = System.IO.Path.GetFileNameWithoutExtension(mPath);
            mFriendlyName = "Currency application :)";
            mImage = ImageHelper.ImageToByteArray(Resources.currency_icon);
        }

        public UserControl Control
        {
            get { return mControl;}
            set { mControl = (CurrencyAppControl)value; }
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
            mControl = new CurrencyAppControl();
        }

        public void StopApplication()
        {
            
        }
    }
}
