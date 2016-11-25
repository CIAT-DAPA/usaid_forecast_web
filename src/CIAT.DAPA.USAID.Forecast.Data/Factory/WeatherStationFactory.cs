using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Factory
{
    /// <summary>
    /// This class allow to get information about weather station collection
    /// </summary>
    public class WeatherStationFactory: FactoryDB<WeatherStation>
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public WeatherStationFactory(IMongoDatabase database): base(database, LogEntity.lc_weather_station)
        {

        }

        public async override Task<bool> updateAsync(WeatherStation entity, WeatherStation newEntity)
        {
            newEntity.track = entity.track;
            newEntity.track.updated = DateTime.Now;
            var result = await collection.ReplaceOneAsync(Builders<WeatherStation>.Filter.Eq("_id", entity.id), newEntity);
            return true;
        }

        public async override Task<bool> deleteAsync(WeatherStation entity)
        {
            await collection.UpdateOneAsync(Builders<WeatherStation>.Filter.Eq("_id", entity.id), Builders<WeatherStation>.Update.Set("track.enable", false)
                                                                                               .Set("track.updated", DateTime.Now));
            return true;
        }

        public async override Task<WeatherStation> insertAsync(WeatherStation entity)
        {
            entity.track = new Track() { enable = true, register = DateTime.Now, updated = DateTime.Now };
            await collection.InsertOneAsync(entity);
            return entity;
        }
    }
}
