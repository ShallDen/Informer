using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using System.Configuration;
using OpenWeatherMap;
using Informer.DataAccess;

namespace WeatherService
{
    public class WeatherService : IWeatherService
    {
        private static WeatherService instance;
        private static OpenWeatherMapClient mWeatherClient;

        private static int mUserCityId;
        private static MetricSystem mUserMetricSystem;
        private static OpenWeatherMapLanguage mUserLanguage;

        private static List<int> seekList = new List<int>();

        public static WeatherService Instance
        {
            get { return instance ?? (instance = new WeatherService()); }
        }

        static WeatherService()
        {
            var mAppId = ConfigurationManager.AppSettings["ApplicationId"];
            mWeatherClient = new OpenWeatherMapClient(mAppId);

            Timer mWeatherSeekTimer = new Timer();
            //mWeatherSeekTimer.Interval = Double.Parse(ConfigurationManager.AppSettings["WeatherUpdateFromWebInterval"], System.Globalization.CultureInfo.InvariantCulture);
            mWeatherSeekTimer.Interval = 10000;
            mWeatherSeekTimer.Elapsed += mWeatherSeekTimer_Elapsed;

            mUserCityId = Int32.Parse(ConfigurationManager.AppSettings["DefaultCityId"], System.Globalization.CultureInfo.InvariantCulture);
            mUserMetricSystem = MetricSystem.Metric;
            mUserLanguage = OpenWeatherMapLanguage.EN;

          //  seekList.Add(mUserCityId);
            mWeatherSeekTimer.Start();
        }

        private WeatherService() { }

        public void StartSeek(int cityId, MetricSystem metricSystem, OpenWeatherMapLanguage language)
        {
            mUserCityId = cityId;
            mUserMetricSystem = metricSystem;
            mUserLanguage = language;

            if (!IsSeekListContainsCity(cityId))
            {
                lock (seekList)
                {
                    seekList.Add(cityId);
                }
            }

            //TO COMMENT THIS
           // SeekWeather();
              
        }
        private bool IsSeekListContainsCity(int cityId)
        {
            return seekList.Where(k => k == cityId).Any();
        }

        private static void mWeatherSeekTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SeekWeather();
        }

        private static void SeekWeather()
        {
            lock (seekList)
            {
                foreach (var city in seekList)
                {
                    var currentWeather = mWeatherClient.CurrentWeather.GetByCityId(city, mUserMetricSystem, mUserLanguage);
                    WriteWeatherInfoToDatabase(currentWeather);
                }
            }
           
        }

        private static void WriteWeatherInfoToDatabase(Task<CurrentWeatherResponse> currentWeather)
        {
            using (InformerContext context = new InformerContext())
            {
                context.WeatherItems.Add(currentWeather.Result);
                context.SaveChanges();
            }
        }

        //public Task<CurrentWeatherResponse> GetWeatherFromWeb(int cityId, MetricSystem metricSystem, OpenWeatherMapLanguage language)
        //{
        //    var currentWeather = mWeatherClient.CurrentWeather.GetByCityId(cityId, metricSystem, language);
        //    return currentWeather;
        //}
    }
}







//var currentWeather = client.CurrentWeather.GetByCityId(1151254, MetricSystem.Metric, OpenWeatherMapLanguage.EN); //Phuket
//var currentWeather = client.CurrentWeather.GetByCityId(532715, MetricSystem.Metric, OpenWeatherMapLanguage.EN);  //Lyskovo
//var currentWeather = client.CurrentWeather.GetByCityId(5128581, MetricSystem.Metric, OpenWeatherMapLanguage.EN); //NY
//var currentWeather = client.CurrentWeather.GetByCityId(6058560, MetricSystem.Metric, OpenWeatherMapLanguage.EN); //London
