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
using OpenWeatherMap;
using System.Threading;
using WeatherApp.StorageServiceReference;
using WeatherApp.WeatherServiceReference;
using System.ServiceModel;

namespace WeatherApp
{
    /// <summary>
    /// Interaction logic for ApplicationControl.xaml
    /// </summary>
    public partial class ApplicationControl : UserControl, WeatherServiceCallback
    {
        private readonly StorageServiceClient storageClient;
        private readonly WeatherServiceClient weatherClient;
        private object lockObj;
        private Guid mGuid;


        public ApplicationControl()
        {
            InitializeComponent();

            lockObj = new object();
            mGuid = Guid.NewGuid();

            storageClient = new StorageServiceClient();

            InstanceContext context = new InstanceContext(this);
            weatherClient = new WeatherServiceClient(context);

            Thread thread = new Thread(() => ShowWeather(null));
            thread.Start();

            //Test();
        }

        public void OnWeatherReceived(WeatherItem weather)
        {
            ShowWeather(weather);
        }

        private void btnGetWeather_Click(object sender, RoutedEventArgs e)
        {
            ShowWeather(null);
        }

        public void ShowWeather(WeatherItem weatherItem)
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

            if (currentCityId != 0)
            {
                var cityModels = storageClient.GetCityModels(CitySearchType.ByCityId, currentCityId.ToString());
                currentCityName = cityModels.First().name;
            }

            if (weatherItem == null)
            {
                weatherItem = storageClient.GetWeatherFromDbByCity(currentCityName);
            }

            if (weatherItem == null)
                return;

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
                txtServerUpdateDateTime.Text = "Last web update at " + weatherItem.LastUpdate.Value.AddHours(3).ToString();
            }));

        }

        private void btnGetNewWeather_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CurrentWeatherRequest currentWeatherRequest = new CurrentWeatherRequest();
                currentWeatherRequest.CityId = 520555;

                // NN 520555
                // Phuket 1151254
                // Moscow 524901

                if (!string.IsNullOrEmpty(txtCitiId.Text))
                    currentWeatherRequest.CityId = int.Parse(txtCitiId.Text);

                currentWeatherRequest.UserMetricSystem = MetricSystem.Metric;
                currentWeatherRequest.UserLanguage = OpenWeatherMapLanguage.EN;
                weatherClient.RegisterClient(mGuid, currentWeatherRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }



        public void Test()
        {
            // weatherClient.RegisterClient(_guid);
            // weatherClient.UnRegisterClient(_guid);

            //var cityModels = storageClient.GetCityModels(CitySearchType.ByCityName, "Moscow");
            //var cityModelsId = storageClient.GetCityModels(CitySearchType.ByCityId, "520555");




            //string filePath = "../../../OpenWeatherMap.Net45/Models/CityList.json";
            //var cityListModel = SerializationHelper.DeserializeJsonFromFile<CityListModel>(filePath, new CityListModel());

            //var city = cityListModel.cityList.Where(c => c.name == "Moscow").ToList();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            weatherClient.UnRegisterClient(mGuid);
        }
    }
}
