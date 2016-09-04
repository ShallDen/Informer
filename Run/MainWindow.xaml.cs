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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Http;
using System.Data.Entity;
using System.Timers;
using System.Configuration;
using OpenWeatherMap;
using Run.WeatherServiceReference;
using Run.StorageServiceReference;
using System.Threading;
using Timer = System.Timers.Timer;
using Informer.Utils;

namespace Run
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Timer mWeatherSeekTimer;
        private readonly StorageServiceClient storageClient;
        private object lockObj;

        public MainWindow()
        {
            InitializeComponent();

            lockObj = new object();
            mWeatherSeekTimer = new Timer();
            var interval = ConfigurationManager.AppSettings["WeatherUpdateFromDbInterval"];
            mWeatherSeekTimer.Interval = Double.Parse(interval, System.Globalization.CultureInfo.InvariantCulture);
            mWeatherSeekTimer.Elapsed += WeatherSeekTimer_Elapsed;
            mWeatherSeekTimer.Start();

            storageClient = new StorageServiceClient();
            Thread thread = new Thread(RequestWeather);
            thread.Start();

         //   Test();
        }

        private void btnGetWeather_Click(object sender, RoutedEventArgs e)
        {
            RequestWeather();
        }

        private void WeatherSeekTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            RequestWeather();
        }

        private void RequestWeather()
        {
            lock (lockObj)
            {
                string currentCityName = string.Empty;
                int currentCityId = 0;

                Dispatcher.Invoke(new Action(() =>
                {
                    if (string.IsNullOrEmpty(txtCityInput.Text))
                        currentCityName = "Nizhniy Novgorod";
                    else
                        currentCityName = txtCityInput.Text;
                    if (!string.IsNullOrEmpty(txtCitiId.Text))
                        currentCityId = int.Parse(txtCitiId.Text);
                }));

                //var temp = storageClient.GetSum(1, 3);

                if (currentCityId!=0)
                {
                    string filePath = "../../../OpenWeatherMap.Net45/Models/cityList.json";
                    var cityListModel = SerializationHelper.DeserializeFromFile(filePath);

                    var city = cityListModel.cityList.Where(c => c._id == currentCityId).SingleOrDefault();
                    currentCityName = city.name;
                }
                
                var weatherItem = storageClient.GetWeatherFromDbByCity(currentCityName);

                Dispatcher.Invoke(new Action(() =>
                {
                    if (weatherItem != null)
                    {
                        this.txtCity.Text = weatherItem.City.Name;
                        this.txtTemperature.Text = weatherItem.Temperature.Value.ToString() + @" °C";
                        this.txtIcon.Text = weatherItem.Weather.Value;
                        this.txtPressure.Text = weatherItem.Pressure.Value.ToString() + " hPa";
                    }

                    txtUpdateDateTime.Text = "Last update at " + DateTime.Now.ToString();
                }));
            }
        }

        private void btnGetNewWeather_Click(object sender, RoutedEventArgs e)
        {
            WeatherServiceClient client = new WeatherServiceClient();
            CurrentWeatherRequest currentWeatherRequest = new CurrentWeatherRequest();
            currentWeatherRequest.CityId = 520555;
            if (!string.IsNullOrEmpty(txtCitiId.Text))
                currentWeatherRequest.CityId = int.Parse(txtCitiId.Text);
            // NN 520555
            // Phuket 1151254
            // Moscow 524901

            currentWeatherRequest.UserMetricSystem = MetricSystem.Metric;
            currentWeatherRequest.UserLanguage = OpenWeatherMapLanguage.EN;
            client.StartSeek(currentWeatherRequest);
        }











        public void Test()
        {
            string filePath = "../../../OpenWeatherMap.Net45/Models/cityList.json";
            var cityListModel = SerializationHelper.DeserializeFromFile(filePath);

            var city = cityListModel.cityList.Where(c => c.name == "Moscow").ToList();
            //SERVER SIDE METHOD
            //
            //WeatherServiceClient client = new WeatherServiceClient();

            //var userCity = 520555; // NN 520555
            //                       // 1151254;

            //var metricSystem = MetricSystem.Metric;
            //var language = OpenWeatherMapLanguage.EN;
            //var currentWeather = client.GetWeatherFromWeb(userCity, metricSystem, language);
            ////






            //using (InformerContext context = new InformerContext())
            //{
            //    context.WeatherItems.Add(currentWeather);
            //    context.SaveChanges();

            //    var temp = context.WeatherItems.Include("City").ToList();
            //    foreach(var tempRes in temp)
            //    {
            //        Console.WriteLine(tempRes);
            //    }
            //}

            //CLIENT SIDE METHOD
            //
            //using (InformerContext context = new InformerContext())
            //{
            //    var weatherItem = context.WeatherItems.OrderByDescending(o => o.Id);
            //    var city = weatherItem.Select(c => c.City.Name).First();
            //    var temperature = weatherItem.Select(c => c.Temperature.Value.ToString() + @" °C").First();
            //    var weather = weatherItem.Select(c => c.Weather.Value).First();
            //    var pressure = weatherItem.Select(c => c.Pressure.Value.ToString() + " hPa").First();

            //    this.txtCity.Text = city;
            //    this.txtTemperature.Text = temperature;
            //    this.txtIcon.Text = weather;
            //    this.txtPressure.Text = pressure;
            //}
            //

            //this.txtCity.Text = city;
            //this.txtTemperature.Text = weatherItem.Temperature.Value.ToString() + @" °C";
            //this.txtIcon.Text = weatherItem.Weather.Value;
            //this.txtPressure.Text = weatherItem.Pressure.Value.ToString() + " hPa";

            //InformerContext context = new InformerContext();

            //// Вставить данные в таблицу Customers с помощью LINQ
            //context.WeatherItems.Add(currentWeather);

            //// Сохранить изменения в БД
            //context.SaveChanges();
        }
    }
}
