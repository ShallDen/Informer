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
    /// Interaction logic for ApplicationWindow.xaml
    /// </summary>
    public partial class ApplicationWindow : Window
    {
        public ApplicationWindow()
        {
            InitializeComponent();
        }

        private void ShowContentButton_Click(object sender, RoutedEventArgs e)
        {
            UserControl myControl = null;
            Assembly asm = Assembly.LoadFile(@"C:\Users\dmayasov\Source\Repos\Informer\WeatherApp\bin\Debug\WeatherApp.dll");
            Type[] tlist = asm.GetTypes();
            foreach (Type t in tlist)
            {
                if (t.Name == "ApplicationControl")
                {
                    myControl = Activator.CreateInstance(t) as UserControl;
                    this.ContentMain.Content = myControl;
                }
            }
        }

        private void ShowContentButton2_Click(object sender, RoutedEventArgs e)
        {
            UserControl myControl = null;
            Assembly asm = Assembly.LoadFile(@"C:\Users\dmayasov\Source\Repos\Informer\CurrencyApp\bin\Debug\CurrencyApp.dll");
            Type[] tlist = asm.GetTypes();
            foreach (Type t in tlist)
            {
                if (t.Name == "ApplicationControl")
                {
                    myControl = Activator.CreateInstance(t) as UserControl;
                    this.ContentMain.Content = myControl;
                }
            }
        }
    }
}
