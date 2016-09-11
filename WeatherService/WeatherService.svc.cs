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
    [ServiceBehavior(InstanceContextMode =
        InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class WeatherService : IWeatherService
    {
        private OpenWeatherMapClient mWeatherClient;
        private Timer mWeatherSeekTimer;
        private object clientsLocker;
        private static List<CurrentWeatherRequest> seekList;
        private static Dictionary<Client, IWeatherServiceCallback> clients;

        private WeatherService()
        {
            clientsLocker = new object();

            seekList = new List<CurrentWeatherRequest>();
            clients = new Dictionary<Client, IWeatherServiceCallback>();

            var mAppId = ConfigurationManager.AppSettings["ApplicationId"];
            mWeatherClient = new OpenWeatherMapClient(mAppId);

            mWeatherSeekTimer = new Timer();
            mWeatherSeekTimer.Interval = Double.Parse(ConfigurationManager.AppSettings["WeatherUpdateFromWebInterval"], System.Globalization.CultureInfo.InvariantCulture);
            mWeatherSeekTimer.Interval = 30000;
            mWeatherSeekTimer.Elapsed += mWeatherSeekTimer_Elapsed;
        }

        public void RegisterClient(Guid guid, CurrentWeatherRequest currentWeatherRequest)
        {
            IWeatherServiceCallback callback = OperationContext.Current.GetCallbackChannel<IWeatherServiceCallback>();

            //---prevent multiple clients adding at the same time---
            lock (clientsLocker)
            {
                //TO ADD CHECK IF IT ALREADY IN clients DICTIONARY
                clients.Add(new Client { id = guid, weatherRequest = currentWeatherRequest }, callback);
            }

            if (!IsSeekListContainsCity(currentWeatherRequest))
            {
                lock (seekList)
                {
                    seekList.Add(currentWeatherRequest);
                }

                if (!mWeatherSeekTimer.Enabled)
                {
                    mWeatherSeekTimer.Start();
                    SeekWeather();
                }
            }
        }

        //---unregister a client by removing its GUID from 
        // dictionary---
        public void UnRegisterClient(Guid guid)
        {
            var query = from c in clients.Keys
                        where c.id == guid
                        select c;
            clients.Remove(query.First());
        }

        public void NotifyClients(WeatherItem weatherResult)
        {
            //---get all the clients in dictionary---
            //notify only clients who has cityId same as cityId in weatherResult
            var clientsForUpdate = clients.Where(c => c.Key.weatherRequest.CityId == weatherResult.City.Id).Select(v => v.Value).ToList();

            //---create the callback action---
            Action<IWeatherServiceCallback> action =
                delegate (IWeatherServiceCallback callback)
                {
                    //---callback to pass the seats booked 
                    // by a client to all other clients---        
                    callback.OnWeatherReceived(weatherResult);
                };

            //---for each connected client, invoke the callback--- 
            clientsForUpdate.ForEach(action);
        }     

        private bool IsSeekListContainsCity(CurrentWeatherRequest currentWeatherRequest)
        {
            return seekList.Where(k => k.CityId == currentWeatherRequest.CityId).Any();
        }

        private bool IsSeekListContainsCity(int cityId)
        {
            return seekList.Where(k => k.CityId == cityId).Any();
        }

        private void mWeatherSeekTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SeekWeather();
        }

        private void SeekWeather()
        {
            lock (seekList)
            {
                foreach (var item in seekList)
                {
                    var currentWeather = mWeatherClient.CurrentWeather.GetByCityId(item.CityId, item.UserMetricSystem, item.UserLanguage);
                    NotifyClients(currentWeather.Result);
                   // WriteWeatherInfoToDatabase(currentWeather);
                }
            }
        }

        private void WriteWeatherInfoToDatabase(Task<CurrentWeatherResponse> currentWeather)
        {
            //SerializationHelper.Serialize(@"C:\Users\ShallDen\Desktop\WeatherObject.xml", currentWeather.Result);
            //var weather = (SerializationHelper.Deserialize(@"C:\Users\ShallDen\Desktop\WeatherObject.xml", typeof(CurrentWeatherResponse)) as CurrentWeatherResponse);
            using (InformerContext context = new InformerContext())
            {
                context.WeatherItems.Add(currentWeather.Result);
                context.SaveChanges();
            }
        }
    }
}



//public Task<CurrentWeatherResponse> GetWeatherFromWeb(int cityId, MetricSystem metricSystem, OpenWeatherMapLanguage language)
//{
//    var currentWeather = mWeatherClient.CurrentWeather.GetByCityId(cityId, metricSystem, language);
//    return currentWeather;
//}



//var currentWeather = client.CurrentWeather.GetByCityId(1151254, MetricSystem.Metric, OpenWeatherMapLanguage.EN); //Phuket
//var currentWeather = client.CurrentWeather.GetByCityId(532715, MetricSystem.Metric, OpenWeatherMapLanguage.EN);  //Lyskovo
//var currentWeather = client.CurrentWeather.GetByCityId(5128581, MetricSystem.Metric, OpenWeatherMapLanguage.EN); //NY
//var currentWeather = client.CurrentWeather.GetByCityId(6058560, MetricSystem.Metric, OpenWeatherMapLanguage.EN); //London
