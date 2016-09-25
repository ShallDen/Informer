using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Informer.Core;
using System.Drawing;
using System.IO;
using Informer.Utils;

namespace QuickApp
{
    /// <summary>
    /// Interaction logic for ApplicationWindow.xaml
    /// </summary>
    public partial class ApplicationWindow : Window
    {
        public IInfomerApplication InfomerApplication { get; set; }

        public ApplicationWindow(IInfomerApplication app)
        {
            InitializeComponent();

            this.InfomerApplication = app;
            this.ContentMain.Content = InfomerApplication.Control;
            this.highPanel.Text = InfomerApplication.FriendlyName;

            this.Icon = ImageHelper.ToImage(InfomerApplication.Image);
            this.image.Source = ImageHelper.ToImage(InfomerApplication.Image);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ApplicationManager.Instance.AppWindows.Contains(this))
            {
                ApplicationManager.Instance.AppWindows.Remove(this);
            }
        }
    }
}
