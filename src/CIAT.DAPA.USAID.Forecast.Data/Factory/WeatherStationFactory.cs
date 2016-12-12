using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using MongoDB.Bson;
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
            newEntity.conf_files = entity.conf_files.Count() == 0 ? new List<ConfigurationFile>() : entity.conf_files;
            newEntity.ranges = entity.ranges.Count() == 0 ? new List<YieldRange>() : entity.ranges;
            var result = await collection.ReplaceOneAsync(Builders<WeatherStation>.Filter.Eq("_id", entity.id), newEntity);
            return result.ModifiedCount > 0;
        }

        public async override Task<bool> deleteAsync(WeatherStation entity)
        {
            var result = await collection.UpdateOneAsync(Builders<WeatherStation>.Filter.Eq("_id", entity.id), Builders<WeatherStation>.Update.Set("track.enable", false)
                                                                                               .Set("track.updated", DateTime.Now));
            return result.ModifiedCount > 0;
        }

        public async override Task<WeatherStation> insertAsync(WeatherStation entity)
        {
            entity.track = new Track() { enable = true, register = DateTime.Now, updated = DateTime.Now };
            entity.conf_files = new List<ConfigurationFile>();
            entity.ranges = new List<YieldRange>();
            await collection.InsertOneAsync(entity);
            return entity;
        }

        /// <summary>
        /// Method that return all registers enable in the database
        /// by the state
        /// </summary>
        /// <param name="state">Id of state</param>
        /// <returns>List of the weather stations</returns>
        public async virtual Task<List<WeatherStation>> listEnableByStateAsync(ObjectId state)
        {
            // Filter all entities available.
            var municipalities = db.GetCollection<Municipality>(Enum.GetName(typeof(LogEntity), LogEntity.lc_municipality))
                                .AsQueryable().Where(f => f.track.enable).ToList();
            var weatherstations = collection
                                .AsQueryable().Where(f => f.track.enable).ToList();
            // Join all data and groups the data by the state
            var query = from m in municipalities 
                        join w in weatherstations on m.id equals w.municipality
                        where m.state == state
                        select w;
            return query.ToList();
        }

        /// <summary>
        /// Method that return all registers enable in the database
        /// by the extern id
        /// </summary>
        /// <param name="ext_ids">Array of the extern ids</param>
        /// <returns>List of the weather stations</returns>
        public async virtual Task<List<WeatherStation>> listEnableByExtIDsAsync(string[] ext_ids)
        {
            // Filter all entities available.
            var query = from ws in collection.AsQueryable()
                        where ws.track.enable && ext_ids.Contains(ws.ext_id)
                        select ws;
            return query.ToList();
        }

        /// <summary>
        /// Method that return all registers enable in the database
        /// by the name
        /// </summary>
        /// <param name="names">Array of the weather stations names</param>
        /// <returns>List of the weather stations</returns>
        public async virtual Task<List<WeatherStation>> listEnableByNamesAsync(string[] names)
        {
            // Filter all entities available.
            var query = from ws in collection.AsQueryable()
                        where ws.track.enable && names.Contains(ws.name)
                        select ws;
            return query.ToList();
        }
    }
}
