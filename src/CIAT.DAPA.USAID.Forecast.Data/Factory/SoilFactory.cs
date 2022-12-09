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
            newEntity.threshold = entity.threshold == null || entity.threshold.Count() == 0 ? new List<Threshold>() : entity.threshold;
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
        /// Method that add a new threshold to a soil
        /// </summary>
        /// <param name="entity">Soil to add new threshold</param>
        /// <param name="threshold">New threshold to add to the soil</param>
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
        /// Method that delete a threshold entity from a soil
        /// </summary>
        /// <param name="entity">Soil to delete threshold</param>
        /// <param name="label">Name of threshold of the soil</param>
        /// <param name="value">Value of the thresholdr</param>
        /// <returns>True if the entity is updated and threshold deleted, false otherwise</returns>
        public async Task<bool> deleteThresholdAsync(Soil entity, string label, double value)
        {
            List<Threshold> allThreshold = new List<Threshold>();
            // This cicle search the threshold to delete
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
