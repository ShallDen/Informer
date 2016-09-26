using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using OpenWeatherMap;
using Informer.DataAccess;

namespace StorageService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [LoggingServiceBehavior]
    [ServiceBehavior(InstanceContextMode =
        InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Multiple,
        UseSynchronizationContext = false)]
    public class StorageService : IStorageService
    {
        public StorageService()
        {

        }

        public int GetSum(int x, int y)
        {
            return x + y;
        }

        public WeatherItem GetWeatherFromDbByCity(string cityName)
        {
            var weathetItems = new List<WeatherItem>();
            using (InformerContext context = new InformerContext())
            {
                weathetItems = context.WeatherItems
                    .Include("City")
                    .Include("City.Sun")
                    .Include("City.Coordinates")
                    .Include("Clouds")
                    .Include("Humidity")
                    .Include("LastUpdate")
                    .Include("Precipitation")
                    .Include("Pressure")
                    .Include("Temperature")
                    .Include("Weather")
                    .Include("Wind")
                    .Include("Wind.Speed")
                    .Include("Wind.Direction")
                    .ToList();
            }

            var currentCity = weathetItems.Where(c => c.City.Name == cityName); //current city
            var lastWeatherItem = currentCity.Where(c => c.LastUpdate.Id == currentCity.Max(m => m.LastUpdate.Id)).SingleOrDefault(); //last weather item

            return lastWeatherItem;
        }

        public List<CityModel> GetCityModels(CitySearchType searchType, string cityParameter)
        {
            CityHelper cityHelper = new CityHelper();
            return cityHelper.GetCityModels(searchType, cityParameter);
        }
    }
}
