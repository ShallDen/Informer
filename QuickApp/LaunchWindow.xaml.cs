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
using System.Reflection;
using System.IO;
using Informer.Core;

namespace QuickApp
{
    /// <summary>
    /// Логика взаимодействия для LaunchWindow.xaml
    /// </summary>
    public partial class LaunchWindow : Window
    {
        public LaunchWindow()
        {
            InitializeComponent();

            ApplicationManager.Instance.LoadApplications();
        }

        private void btnApp_Click(object sender, RoutedEventArgs e)
        {
            var buttonContent = (sender as Button).Content.ToString();
            var application = ApplicationManager.Instance.Applications.FirstOrDefault(app => app.AssemblyName == buttonContent);

            if (application != null)
            {
                application.RunApplication();

                var appWindow = new ApplicationWindow(application);
                appWindow.Show();
            }
        }
    }
}
