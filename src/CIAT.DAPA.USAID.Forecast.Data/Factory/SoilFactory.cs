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
    /// This class allow to get information about soils collection
    /// </summary>
    public class SoilFactory: FactoryDB<Soil>
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public SoilFactory(IMongoDatabase database): base(database, LogEntity.cp_soil)
        {

        }

        public async override Task<bool> updateAsync(Soil entity, Soil newEntity)
        {
            newEntity.track = entity.track;
            newEntity.track.updated = DateTime.Now;
            var result = await collection.ReplaceOneAsync(Builders<Soil>.Filter.Eq("_id", entity.id), newEntity);
            return result.ModifiedCount > 0;
        }

        public async override Task<bool> deleteAsync(Soil entity)
        {
            var result = await collection.UpdateOneAsync(Builders<Soil>.Filter.Eq("_id", entity.id), Builders<Soil>.Update.Set("track.enable", false)
                                                                                               .Set("track.updated", DateTime.Now));
            return result.ModifiedCount > 0;
        }

        public async override Task<Soil> insertAsync(Soil entity)
        {
            entity.track = new Track() { enable = true, register = DateTime.Now, updated = DateTime.Now };
            await collection.InsertOneAsync(entity);
            return entity;
        }

        public async override Task<List<Soil>> listEnableAsync()
        {
            var builder = Builders<Soil>.Filter;
            var filter = builder.Eq("track.enable", true);
            return await collection.Find(filter).SortByDescending(p => p.order).ToListAsync<Soil>();
        }

        /// <summary>
        /// Method that add a new setup to a crop
        /// </summary>
        /// <param name="entity">Weather station with the new range</param>
        /// <param name="range">New range to add to the weather station</param>
        /// <returns>True if the entity is updated, false otherwise</returns>
        public async Task<bool> addThresholdAsync(Soil entity, Threshold threshold)
        {
            List<Threshold> allThreshold = new List<Threshold>();
            if(entity.threshold != null)
            {
                allThreshold = entity.threshold.ToList();
            }
            allThreshold.Add(threshold);
            entity.threshold = allThreshold;
            var result = await collection.UpdateOneAsync(Builders<Soil>.Filter.Eq("_id", entity.id), Builders<Soil>.Update.Set("threshold", entity.threshold));
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
        public async Task<bool> deleteThresholdAsync(Soil entity, string label, double value)
        {
            List<Threshold> allThreshold = new List<Threshold>();
            // This cicle search the range to delete
            foreach (var t in entity.threshold)
            {
                // If the setup is found, it will update
                if (!(t.label.Equals(label) && t.value == value))
                    allThreshold.Add(t);
            }
            entity.threshold = allThreshold;
            var result = await collection.UpdateOneAsync(Builders<Soil>.Filter.Eq("_id", entity.id), Builders<Soil>.Update.Set("threshold", entity.threshold));
            return result.ModifiedCount > 0;
        }
    }
}
