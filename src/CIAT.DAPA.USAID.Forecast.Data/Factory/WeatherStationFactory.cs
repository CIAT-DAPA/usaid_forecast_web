using CIAT.DAPA.USAID.Forecast.Data.Database;
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

        public async Task<bool> updateWeatherRangeAsync(WeatherStation entity, List<YieldRange> range)
        {
 
            if (range != null)
            {
                entity.ranges = range;
            }
            entity.track.updated = DateTime.Now;
            entity.conf_files = entity.conf_files.Count() == 0 ? new List<ConfigurationFile>() : entity.conf_files;
            var result = await collection.ReplaceOneAsync(Builders<WeatherStation>.Filter.Eq("_id", entity.id), entity);
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// Method that add a new setup to a crop
        /// </summary>
        /// <param name="entity">Weather station with the new range</param>
        /// <param name="range">New range to add to the weather station</param>
        /// <returns>True if the entity is updated, false otherwise</returns>
        public async Task<bool> addRangeAsync(WeatherStation entity, YieldRange range)
        {            
            List<YieldRange> allRanges = entity.ranges.ToList();
            allRanges.Add(range);
            entity.ranges = allRanges;
            var result = await collection.UpdateOneAsync(Builders<WeatherStation>.Filter.Eq("_id", entity.id), Builders<WeatherStation>.Update.Set("ranges", entity.ranges));
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// Method that delete a yield range entity from a weather station
        /// </summary>
        /// <param name="entity">Crop to delete the setup configuration</param>
        /// <param name="crop">Id of the crop</param>
        /// <param name="label">Name of level</param>
        /// <param name="lower">Limit lower</param>
        /// <param name="upper">Limit upper</param>
        /// <returns>True if the entity is updated, false otherwise</returns>
        public async Task<bool> deleteRangeAsync(WeatherStation entity, string crop, string label, int lower, int upper)
        {
            List<YieldRange> allRanges = new List<YieldRange>();
            // This cicle search the range to delete
            foreach (var r in entity.ranges)
            {
                // If the setup is found, it will update
                if (!(r.crop == ForecastDB.parseId(crop) && r.label.Equals(label) && r.lower == lower && r.upper == upper))
                    allRanges.Add(r);
            }
            entity.ranges = allRanges;
            var result = await collection.UpdateOneAsync(Builders<WeatherStation>.Filter.Eq("_id", entity.id), Builders<WeatherStation>.Update.Set("ranges", entity.ranges));
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// Method that add a new setup to a crop
        /// </summary>
        /// <param name="entity">Weather station with the new range</param>
        /// <param name="file">New range to add to the weather station</param>
        /// <returns>True if the entity is updated, false otherwise</returns>
        public async Task<bool> addConfigurationFileAsync(WeatherStation entity, ConfigurationFile file)
        {
            List<ConfigurationFile> allFiles = entity.conf_files.ToList();
            allFiles.Add(file);
            entity.conf_files = allFiles;
            var result = await collection.UpdateOneAsync(Builders<WeatherStation>.Filter.Eq("_id", entity.id), Builders<WeatherStation>.Update.Set("conf_files", entity.conf_files));
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

        public async virtual Task<List<WeatherStation>> listEnableByMunicipalityAsync(ObjectId municipality)
        {

            var weatherstations = collection
                                .AsQueryable().Where(f => f.track.enable).ToList();
            // Join all data and groups the data by the state
            var query = from w in weatherstations
                        where w.municipality == municipality
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
        /// by the id list
        /// </summary>
        /// <param name="ids">Array of the ids</param>
        /// <returns>List of the weather stations</returns>
        public async virtual Task<List<WeatherStation>> listEnableByIDsAsync(ObjectId[] ids)
        {
            // Filter all entities available.
            var query = from ws in collection.AsQueryable()
                        where ws.track.enable && ids.Contains(ws.id)
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

        /// <summary>
        /// Method that return all registers enable and visible in the database
        /// </summary>
        /// <returns>List of the weather stations</returns>
        public async virtual Task<List<WeatherStation>> listEnableVisibleAsync()
        {
            var builder = Builders<WeatherStation>.Filter;
            var filter = builder.Eq("track.enable", true) & builder.Eq("visible", true);
            return await collection.Find(filter).ToListAsync<WeatherStation>();
        }
    }
}
