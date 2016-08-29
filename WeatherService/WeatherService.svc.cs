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
        private readonly OpenWeatherMapClient mWeatherClient;
        private readonly string mAppId;
        private readonly Timer mWeatherSeekTimer;

        private int mUserCityId;
        private MetricSystem mUserMetricSystem;
        private OpenWeatherMapLanguage mUserLanguage;

        public WeatherService()
        {
            mAppId = ConfigurationManager.AppSettings["ApplicationId"];
            mWeatherClient = new OpenWeatherMapClient(mAppId);

            mWeatherSeekTimer = new Timer();
            mWeatherSeekTimer.Interval = Double.Parse(ConfigurationManager.AppSettings["WeatherUpdateFromWebInterval"], System.Globalization.CultureInfo.InvariantCulture);
            mWeatherSeekTimer.Elapsed += mWeatherSeekTimer_Elapsed;

            mUserCityId = Int32.Parse(ConfigurationManager.AppSettings["DefaultCityId"], System.Globalization.CultureInfo.InvariantCulture);
            mUserMetricSystem = MetricSystem.Metric;
            mUserLanguage = OpenWeatherMapLanguage.EN;
        }

        public void StartSeek(int cityId, MetricSystem metricSystem, OpenWeatherMapLanguage language)
        {
            mUserCityId = cityId;
            mUserMetricSystem = metricSystem;
            mUserLanguage = language;

            SeekWeather();
            mWeatherSeekTimer.Start();
        }

        private void mWeatherSeekTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SeekWeather();
        }

        private void SeekWeather()
        {
            var currentWeather = mWeatherClient.CurrentWeather.GetByCityId(mUserCityId, mUserMetricSystem, mUserLanguage);
            WriteWeatherInfoToDatabase(currentWeather);
        }

        private void WriteWeatherInfoToDatabase(Task<CurrentWeatherResponse> currentWeather)
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
//var currentWeather = client.CurrentWeather.GetByCityId(532715, MetricSystem.Metric, OpenWeatherMapLanguage.EN); //Lyskovo
// var currentWeather = client.CurrentWeather.GetByCityId(5128581, MetricSystem.Metric, OpenWeatherMapLanguage.EN); //NY
//var currentWeather = client.CurrentWeather.GetByCityId(6058560, MetricSystem.Metric, OpenWeatherMapLanguage.EN); //London
