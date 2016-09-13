using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeatherMap
{
    public class CurrentWeatherRequest
    {
        public int CityId { get; set; }
        public MetricSystem UserMetricSystem { get; set; } = MetricSystem.Metric;
        public OpenWeatherMapLanguage UserLanguage { get; set; } = OpenWeatherMapLanguage.EN;
        public DateTime LastUpdate { get; set; } = DateTime.MinValue;
    }
}
