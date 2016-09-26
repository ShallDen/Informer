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
    [ServiceContract(
        Name = "WeatherService",
        Namespace = "http://www.learn2develop.net/",
        CallbackContract = typeof(IWeatherServiceCallback),
        SessionMode = SessionMode.Required)]
    public interface IWeatherService
    {
      //  [OperationContract(IsOneWay = true)]
      //  void StartSeek(CurrentWeatherRequest currentWeatherRequest);

        [OperationContract(IsOneWay = true)]
        void RegisterClient(Guid guid, CurrentWeatherRequest currentWeatherRequest);

        [OperationContract(IsOneWay = true)]
        void UnRegisterClient(Guid id);

        //[OperationContract]
        //Task<CurrentWeatherResponse> GetWeatherFromWeb(int cityId, MetricSystem metricSystem, OpenWeatherMapLanguage language);
    }

    public interface IWeatherServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void OnWeatherReceived(WeatherItem weather);
    }

    [DataContract]
    public class Client
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public CurrentWeatherRequest WeatherRequest { get; set; }
    }
}
