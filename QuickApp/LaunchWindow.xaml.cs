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
using System.Reflection;

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
        }

        private void btnCurrencyApp_Click(object sender, RoutedEventArgs e)
        {
            var appWindow = new ApplicationWindow();

            UserControl myControl = null;
            Assembly asm = Assembly.LoadFile(@"F:\Programming\Informer\Informer\CurrencyApp\bin\Debug\CurrencyApp.dll");
            Type[] tlist = asm.GetTypes();
            foreach (Type t in tlist)
            {
                if (t.Name == "ApplicationControl")
                {
                    myControl = Activator.CreateInstance(t) as UserControl;
                    appWindow.ContentMain.Content = myControl;
                }
            }

            appWindow.Show();
        }

        private void btnWeatheApp_Click(object sender, RoutedEventArgs e)
        {
            var appWindow = new ApplicationWindow();

            UserControl myControl = null;
            Assembly asm = Assembly.LoadFrom(@"F:\Programming\Informer\Informer\WeatherApp\bin\Debug\WeatherApp.dll");
            Type[] tlist = asm.GetTypes();
            foreach (Type t in tlist)
            {
                if (t.Name == "ApplicationControl")
                {
                    myControl = Activator.CreateInstance(t) as UserControl;
                    appWindow.ContentMain.Content = myControl;
                }
            }

            appWindow.Show();
        }
    }
}
