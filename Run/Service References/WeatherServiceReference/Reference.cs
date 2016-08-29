﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Run.WeatherServiceReference {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="WeatherServiceReference.IWeatherService")]
    public interface IWeatherService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWeatherService/StartSeek", ReplyAction="http://tempuri.org/IWeatherService/StartSeekResponse")]
        void StartSeek(int cityId, OpenWeatherMap.MetricSystem metricSystem, OpenWeatherMap.OpenWeatherMapLanguage language);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWeatherService/StartSeek", ReplyAction="http://tempuri.org/IWeatherService/StartSeekResponse")]
        System.Threading.Tasks.Task StartSeekAsync(int cityId, OpenWeatherMap.MetricSystem metricSystem, OpenWeatherMap.OpenWeatherMapLanguage language);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IWeatherServiceChannel : Run.WeatherServiceReference.IWeatherService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class WeatherServiceClient : System.ServiceModel.ClientBase<Run.WeatherServiceReference.IWeatherService>, Run.WeatherServiceReference.IWeatherService {
        
        public WeatherServiceClient() {
        }
        
        public WeatherServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public WeatherServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WeatherServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WeatherServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void StartSeek(int cityId, OpenWeatherMap.MetricSystem metricSystem, OpenWeatherMap.OpenWeatherMapLanguage language) {
            base.Channel.StartSeek(cityId, metricSystem, language);
        }
        
        public System.Threading.Tasks.Task StartSeekAsync(int cityId, OpenWeatherMap.MetricSystem metricSystem, OpenWeatherMap.OpenWeatherMapLanguage language) {
            return base.Channel.StartSeekAsync(cityId, metricSystem, language);
        }
    }
}
