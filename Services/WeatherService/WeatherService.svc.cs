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
        ConcurrencyMode = ConcurrencyMode.Multiple, 
        UseSynchronizationContext = false)]
    public class WeatherService : IWeatherService
    {
        private OpenWeatherMapClient mWeatherClient;
        private Timer mWeatherSeekTimer;
        private object clientsLocker;
        private static List<CurrentWeatherRequest> cityList;
        private static Dictionary<Client, IWeatherServiceCallback> clientList;

        private WeatherService()
        {
            clientsLocker = new object();

            cityList = new List<CurrentWeatherRequest>();
            clientList = new Dictionary<Client, IWeatherServiceCallback>();

            var mAppId = ConfigurationManager.AppSettings["ApplicationId"];
            mWeatherClient = new OpenWeatherMapClient(mAppId);

            mWeatherSeekTimer = new Timer();
            mWeatherSeekTimer.Interval = Double.Parse(ConfigurationManager.AppSettings["WeatherUpdateFromWebInterval"], System.Globalization.CultureInfo.InvariantCulture);
            mWeatherSeekTimer.Elapsed += mWeatherSeekTimer_Elapsed;
        }

        public void RegisterClient(Guid guid, CurrentWeatherRequest currentWeatherRequest)
        {
            IWeatherServiceCallback callback = OperationContext.Current.GetCallbackChannel<IWeatherServiceCallback>();

            // Prevent multiple clients adding at the same time
            lock (clientsLocker)
            {
                if (!IsClientRegistered(guid))
                {
                    var client = new Client { Id = guid, WeatherRequest = currentWeatherRequest };
                    clientList.Add(client, callback);

                    CurrentWeatherResponse weatherResult = null;
                    try
                    {
                        // Get new weather item for one client who changed cityId
                        var weather = GetWeatherFromWeb(currentWeatherRequest.CityId, currentWeatherRequest.UserMetricSystem, currentWeatherRequest.UserLanguage);
                        weatherResult = weather.Result;

                        WriteWeatherInfoToDatabase(weather);
                    }
                    catch (AggregateException ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }

                    // Notify client to update UI
                    NotifyClient(client, weatherResult);
                }
                else
                {
                    // Get client with specified GUID
                    var client = clientList.First(c => c.Key.Id == guid);

                    // Save cityId for city cleanup check
                    var clientCityId = client.Key.WeatherRequest.CityId;

                    if (clientCityId != currentWeatherRequest.CityId)
                    {
                        // User changed city
                        client.Key.WeatherRequest.CityId = currentWeatherRequest.CityId;
                        currentWeatherRequest.LastUpdate = DateTime.MinValue;

                        // Check if it is required to remove old cityId form cityList
                        CheckCityListForCleanUp(clientCityId);

                        CurrentWeatherResponse weatherResult = null;
                        try
                        {
                            // Get new weather item for one client who changed cityId
                            var weather = GetWeatherFromWeb(currentWeatherRequest.CityId, currentWeatherRequest.UserMetricSystem, currentWeatherRequest.UserLanguage);
                            weatherResult = weather.Result;

                            WriteWeatherInfoToDatabase(weather);
                        }
                        catch (AggregateException ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }

                        // Notify client to update UI
                        NotifyClient(client, weatherResult);
                    }
                }
            }

            if (!IsSeekListContainsCity(currentWeatherRequest))
            {
                lock (cityList)
                {
                    // Create new weather request
                    var newRequest = new CurrentWeatherRequest();

                    newRequest.CityId = currentWeatherRequest.CityId;
                    newRequest.LastUpdate = currentWeatherRequest.LastUpdate;
                    newRequest.UserLanguage = currentWeatherRequest.UserLanguage;
                    newRequest.UserMetricSystem = currentWeatherRequest.UserMetricSystem;

                    cityList.Add(newRequest);
                }

                // Start seek weather at the beginning
                if (!mWeatherSeekTimer.Enabled)
                {
                    mWeatherSeekTimer.Start();
                    SeekWeather();
                }
            }
        }

        // Unregister a client by removing its GUID from dictionary
        public void UnRegisterClient(Guid guid)
        {
            try
            {
                var query = from c in clientList.Keys
                            where c.Id == guid
                            select c;

                if (query != null)
                { 
                    //get city for cleanup check
                    var cityId = query.First().WeatherRequest.CityId;
                    clientList.Remove(query.First());

                    CheckCityListForCleanUp(cityId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        // Check if we need to remove city which isn't used by clients
        private void CheckCityListForCleanUp(int cityId)
        {
            // Find clients who has cleanup cityid
            // If there no such clients, it removes this city from seek cityList
            bool isClientsExists = clientList.Keys.Any(c => c.WeatherRequest.CityId == cityId);

            if (!isClientsExists)
            {
                lock (cityList)
                {
                    var city = cityList.First(c => c.CityId == cityId);
                    cityList.Remove(city);
                }
            }
        }

        private bool IsClientRegistered(Guid guid)
        {
            var result = clientList.Keys.FirstOrDefault(c => c.Id == guid);
            return result != null;
        }

        private bool IsSeekListContainsCity(CurrentWeatherRequest currentWeatherRequest)
        {
            return cityList.Where(k => k.CityId == currentWeatherRequest.CityId).Any();
        }

        private bool IsSeekListContainsCity(int cityId)
        {
            return cityList.Where(k => k.CityId == cityId).Any();
        }

        private void mWeatherSeekTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SeekWeather();
        }

        private void SeekWeather()
        {
            lock (cityList)
            {
                foreach (var item in cityList)
                {
                    try
                    {
                        var currentWeather = mWeatherClient.CurrentWeather.GetByCityId(item.CityId, item.UserMetricSystem, item.UserLanguage);
                        var currentWeatherResult = currentWeather.Result;

                        // Save to DB and notify clients if it is really new weather update
                        if (item.LastUpdate < currentWeatherResult.LastUpdate.Value)
                        {
                            item.LastUpdate = currentWeatherResult.LastUpdate.Value;

                            var needToUpdateClients = clientList.Where(c => c.Key.WeatherRequest.CityId == currentWeatherResult.City.Id).ToList();
                            while (needToUpdateClients.Any())
                            {
                                NotifyClients(currentWeatherResult, needToUpdateClients);
                            }

                            WriteWeatherInfoToDatabase(currentWeather);
                        }
                    }
                    catch (AggregateException ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
        }

        public void NotifyClients(WeatherItem weatherResult, List<KeyValuePair<Client, IWeatherServiceCallback>> needToUpdateClients)
        {
            IWeatherServiceCallback currentCallback = null;
            try
            {
                // Notify only clients who has cityId same as cityId in weatherResult
                var query = needToUpdateClients.Select(v => v.Value).ToList();

                // Create the callback action
                Action<IWeatherServiceCallback> action =
                    delegate (IWeatherServiceCallback callback)
                    {
                        // Callback to update UI to all other clients  
                        currentCallback = callback;
                        callback.OnWeatherReceived(weatherResult);

                        var updatedClient = needToUpdateClients.FirstOrDefault(c => c.Value == callback);
                        if (!updatedClient.Equals(default(KeyValuePair<Client, IWeatherServiceCallback>)))
                        {
                            needToUpdateClients.Remove(updatedClient);
                        }
                    };

                // For each connected client, invoke the callback
                query.ForEach(action);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex.ToString());
                var clientForDelete = clientList.FirstOrDefault(c => c.Value == currentCallback);
                var cityId = clientForDelete.Key.WeatherRequest.CityId;
                clientList.Remove(clientForDelete.Key);
                //CheckCityListForCleanUp(cityId);

                clientForDelete = needToUpdateClients.FirstOrDefault(c => c.Value == currentCallback);
                needToUpdateClients.Remove(clientForDelete);
            }
            catch (CommunicationObjectAbortedException ex)
            {
                Console.WriteLine(ex.ToString());
                var clientForDelete = clientList.FirstOrDefault(c => c.Value == currentCallback);
                var cityId = clientForDelete.Key.WeatherRequest.CityId;
                clientList.Remove(clientForDelete.Key);
               // CheckCityListForCleanUp(cityId);

                clientForDelete = needToUpdateClients.FirstOrDefault(c => c.Value == currentCallback);
                needToUpdateClients.Remove(clientForDelete);
            }
        }

        public void NotifyClient(KeyValuePair<Client, IWeatherServiceCallback> client, WeatherItem weatherResult)
        {
            try
            {
                var clientForUpdate = clientList.Where(c => c.Key.Id == client.Key.Id).Select(v => v.Value).SingleOrDefault();

                if (clientForUpdate != null)
                {
                    clientForUpdate.OnWeatherReceived(weatherResult);
                }
            }
            catch (CommunicationObjectAbortedException ex)
            {
                Console.WriteLine(ex.ToString());
                // clientList.Clear();
            }
        }

        public void NotifyClient(Client client, WeatherItem weatherResult)
        {
            try
            {
                var clientForUpdate = clientList.Where(c => c.Key.Id == client.Id).Select(v => v.Value).SingleOrDefault();

                if (clientForUpdate != null)
                {
                    clientForUpdate.OnWeatherReceived(weatherResult);
                }
            }
            catch (CommunicationObjectAbortedException ex)
            {
                Console.WriteLine(ex.ToString());
                // clientList.Clear();
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

        public Task<CurrentWeatherResponse> GetWeatherFromWeb(int cityId, MetricSystem metricSystem, OpenWeatherMapLanguage language)
        {
            var currentWeather = mWeatherClient.CurrentWeather.GetByCityId(cityId, metricSystem, language);
            return currentWeather;
        }
    }
}







//var currentWeather = client.CurrentWeather.GetByCityId(1151254, MetricSystem.Metric, OpenWeatherMapLanguage.EN); //Phuket
//var currentWeather = client.CurrentWeather.GetByCityId(532715, MetricSystem.Metric, OpenWeatherMapLanguage.EN);  //Lyskovo
//var currentWeather = client.CurrentWeather.GetByCityId(5128581, MetricSystem.Metric, OpenWeatherMapLanguage.EN); //NY
//var currentWeather = client.CurrentWeather.GetByCityId(6058560, MetricSystem.Metric, OpenWeatherMapLanguage.EN); //London
