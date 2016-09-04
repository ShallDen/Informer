using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeatherMap
{
    public class Coord
    {
        public double lon { get; set; }
        public double lat { get; set; }
    }

    public class CityModel
    {
        public int _id { get; set; }
        public string name { get; set; }
        public string country { get; set; }
        public Coord coord { get; set; }
    }

    public class CityListModel
    {
        public List<CityModel> cityList { get; set; }
    }
}
