using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Xml.Serialization;
using OpenWeatherMap;

namespace Informer.Utils
{
    public class SerializationHelper
    {
        public static void Serialize(string filename, Object obj)
        {
            try
            {
                XmlSerializer xmlFormat = new XmlSerializer(obj.GetType());

                using (var fStream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    xmlFormat.Serialize(fStream, obj);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static Object Deserialize(string filename, Type type)
        {
            try
            {
                XmlSerializer xmlFormat = new XmlSerializer(type);
                using (var fStream = File.OpenRead(filename))
                {
                    return xmlFormat.Deserialize(fStream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static CityListModel DeserializeFromFile(string filePath)
        {
            //string json = File.ReadAllText(filePath);
            //string newStr = json.Replace("\n", ",\n");

            //   File.WriteAllText(filePath, newStr);

            CityListModel cityModel = new CityListModel();
            using (StreamReader file = File.OpenText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                cityModel = (CityListModel)serializer.Deserialize(file, typeof(CityListModel));
              //  nn = obj.cityList.Where(c=>c._id==520555).SingleOrDefault();
            }
            return cityModel;
        }
    }
}
