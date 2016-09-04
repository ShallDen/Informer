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
using Informer.Utils;

namespace WeatherService
{
    public class WeatherService : IWeatherService
    {
        private static List<CurrentWeatherRequest> mSeekList;
        private static OpenWeatherMapClient mWeatherClient;

        private WeatherService() { }

        static WeatherService()
        {
            mSeekList = new List<CurrentWeatherRequest>();

            var mAppId = ConfigurationManager.AppSettings["ApplicationId"];
            mWeatherClient = new OpenWeatherMapClient(mAppId);

            Timer mWeatherSeekTimer = new Timer();
            mWeatherSeekTimer.Interval = Double.Parse(ConfigurationManager.AppSettings["WeatherUpdateFromWebInterval"], System.Globalization.CultureInfo.InvariantCulture);
           // mWeatherSeekTimer.Interval = 10000;
            mWeatherSeekTimer.Elapsed += mWeatherSeekTimer_Elapsed;
            mWeatherSeekTimer.Start();
        }

        public void StartSeek(CurrentWeatherRequest currentWeatherRequest)
        {
            if (!IsSeekListContainsCity(currentWeatherRequest))
            {
                lock (mSeekList)
                {
                    mSeekList.Add(currentWeatherRequest);
                }
            }
            //TO COMMENT THIS
           // SeekWeather();
        }

        private bool IsSeekListContainsCity(CurrentWeatherRequest currentWeatherRequest)
        {
            return mSeekList.Where(k => k.CityId == currentWeatherRequest.CityId).Any();
        }

        private static void mWeatherSeekTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SeekWeather();
        }

        private static void SeekWeather()
        {
            lock (mSeekList)
            {
                foreach (var item in mSeekList)
                {
                    var currentWeather = mWeatherClient.CurrentWeather.GetByCityId(item.CityId, item.UserMetricSystem, item.UserLanguage);
                    WriteWeatherInfoToDatabase(currentWeather);
                }
            }
        }

        private static void WriteWeatherInfoToDatabase(Task<CurrentWeatherResponse> currentWeather)
        {
            //SerializationHelper.Serialize(@"C:\Users\ShallDen\Desktop\WeatherObject.xml", currentWeather.Result);
            //var weather = (SerializationHelper.Deserialize(@"C:\Users\ShallDen\Desktop\WeatherObject.xml", typeof(CurrentWeatherResponse)) as CurrentWeatherResponse);
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
