using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Informer.Utils;

namespace OpenWeatherMap
{
    public enum CitySearchType
    {
        ByCityName,
        ByCityId
    }

    public class CityHelper
    {
        public List<CityModel> GetCityModels(CitySearchType searchType, string cityParameter)
        {
            try
            {
                string filePath = @"F:\Programming\Informer\Informer\OpenWeatherMap.Net45\Models\CityList.json";
                var cityListModel = SerializationHelper.DeserializeJsonFromFile<CityListModel>(filePath, new CityListModel());

                var cityModels = new List<CityModel>();

                switch (searchType)
                {
                    case CitySearchType.ByCityName:
                        cityModels = GetCityModelsByName(cityListModel, cityParameter);
                        break;
                    case CitySearchType.ByCityId:
                        cityModels = GetCityModelsById(cityListModel, int.Parse(cityParameter));
                        break;
                    default:
                        cityModels = GetCityModelsByName(cityListModel, cityParameter);
                        break;
                }
                return cityModels;
            }
            catch (Exception)
            {
                return new List<CityModel>();
            }
        }

        private List<CityModel> GetCityModelsByName(CityListModel cityListModel, string cityName)
        {
            return cityListModel.cityList.Where(c => c.name == cityName).ToList();
        }

        private List<CityModel> GetCityModelsById(CityListModel cityListModel, int cityId)
        {
            return cityListModel.cityList.Where(c => c._id == cityId).ToList();
        }
    }
}
