using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Xml.Serialization;

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

        public static T DeserializeJsonFromFile<T>(string filePath, T obj) where T : new()
        {
            //string json = File.ReadAllText(filePath);
            //string newStr = json.Replace("\n", ",\n");

            //   File.WriteAllText(filePath, newStr);

            try
            {
                using (StreamReader file = File.OpenText(filePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    obj = (T)serializer.Deserialize(file, typeof(T));
                    return obj;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new T();
            }
        }
    }
}
