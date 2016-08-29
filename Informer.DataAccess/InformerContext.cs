using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using OpenWeatherMap;

namespace Informer.DataAccess
{
    public class InformerContext : DbContext
    {
        // Имя будущей базы данных можно указать через вызов конструктора базового класса
        public InformerContext() : base("InformerDB")
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<InformerContext>());
            //Database.SetInitializer<InformerContext>(null);
        }

        // Отражение таблиц базы данных на свойства с типом DbSet
         
        public DbSet<City> Cities { get; set; }
        public DbSet<Clouds> Clouds { get; set; }
        public DbSet<Coordinates> Coordinates { get; set; }
        public DbSet<Direction> Directions { get; set; }
        public DbSet<Humidity> Humidities { get; set; }
        public DbSet<LastUpdate> LastUpdates { get; set; }
        public DbSet<Precipitation> Precipitations { get; set; }
        public DbSet<Pressure> Pressures { get; set; }
        public DbSet<Speed> Speeds { get; set; }
        public DbSet<Sun> Suns { get; set; }
        public DbSet<Temperature> Temperatures { get; set; }
        public DbSet<Weather> Weathers { get; set; }
        public DbSet<Wind> Winds { get; set; }
        public DbSet<WeatherItem> WeatherItems { get; set; }
    }
}
