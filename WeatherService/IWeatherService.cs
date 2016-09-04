using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using OpenWeatherMap;

namespace WeatherService
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IService1" в коде и файле конфигурации.
    [ServiceContract]
    public interface IWeatherService
    {
        // TODO: Добавьте здесь операции служб
        [OperationContract]
        void StartSeek(CurrentWeatherRequest currentWeatherRequest);

        //[OperationContract]
        //Task<CurrentWeatherResponse> GetWeatherFromWeb(int cityId, MetricSystem metricSystem, OpenWeatherMapLanguage language);
    }


    // Используйте контракт данных, как показано в примере ниже, чтобы добавить составные типы к операциям служб.
   
}
