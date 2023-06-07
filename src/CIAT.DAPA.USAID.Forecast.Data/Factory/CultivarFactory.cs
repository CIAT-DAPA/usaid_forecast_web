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
    /// This class allow to get information about cultivars collection
    /// </summary>
    public class CultivarFactory: FactoryDB<Cultivar>
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public CultivarFactory(IMongoDatabase database): base(database, LogEntity.cp_cultivar)
        {

        }

        public async override Task<bool> updateAsync(Cultivar entity, Cultivar newEntity)
        {
            newEntity.track = entity.track;
            newEntity.track.updated = DateTime.Now;
            newEntity.threshold = entity.threshold == null || entity.threshold.Count() == 0 ? new List<Threshold>() : entity.threshold;
            var result = await collection.ReplaceOneAsync(Builders<Cultivar>.Filter.Eq("_id", entity.id), newEntity);
            return result.ModifiedCount > 0;
        }

        public async override Task<bool> deleteAsync(Cultivar entity)
        {
            var result = await collection.UpdateOneAsync(Builders<Cultivar>.Filter.Eq("_id", entity.id), Builders<Cultivar>.Update.Set("track.enable", false)
                                                                                               .Set("track.updated", DateTime.Now));
            return result.ModifiedCount > 0;
        }

        public async override Task<Cultivar> insertAsync(Cultivar entity)
        {
            entity.track = new Track() { enable = true, register = DateTime.Now, updated = DateTime.Now };
            await collection.InsertOneAsync(entity);
            return entity;
        }

        /// <summary>
        /// Method that return all records of cultivars available and ordered
        /// </summary>
        /// <returns>List of Cultivars</returns>
        public async override Task<List<Cultivar>> listEnableAsync()
        {
            var builder = Builders<Cultivar>.Filter;
            var filter = builder.Eq("track.enable", true);
            return await collection.Find(filter).SortByDescending(p=> p.order).ToListAsync<Cultivar>();
        }

        /// <summary>
        /// Method that return all records of cultivars available and ordered by crop
        /// </summary>
        /// <param name="crop">ID crop</param>
        /// <returns>List of Cultivars</returns>        
        public async Task<List<Cultivar>> listByCropEnableAsync(ObjectId crop)
        {
            var builder = Builders<Cultivar>.Filter;
            var filter = builder.Eq("track.enable", true) & builder.Eq("crop", crop); ;
            return await collection.Find(filter).SortByDescending(p => p.order).ToListAsync<Cultivar>();
        }


        /// <summary>
        /// Method that add a new threshold to a cultivar
        /// </summary>
        /// <param name="entity">Cultivar to add new threshold</param>
        /// <param name="threshold">New threshold to add to the cultivar</param>
        /// <returns>True if the entity is updated, false otherwise</returns>
        public async Task<bool> addThresholdAsync(Cultivar entity, Threshold threshold)
        {
            List<Threshold> allThreshold = new List<Threshold>();
            if (entity.threshold != null)
            {
                allThreshold = entity.threshold.ToList();
            }
            allThreshold.Add(threshold);
            entity.threshold = allThreshold;
            var result = await collection.UpdateOneAsync(Builders<Cultivar>.Filter.Eq("_id", entity.id), Builders<Cultivar>.Update.Set("threshold", entity.threshold));
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// Method that delete a threshold entity from a cultivar
        /// </summary>
        /// <param name="entity">Cultivar to delete threshold</param>
        /// <param name="label">Name of threshold of the cultivar</param>
        /// <param name="value">Value of the thresholdr</param>
        /// <returns>True if the entity is updated and threshold deleted, false otherwise</returns>
        public async Task<bool> deleteThresholdAsync(Cultivar entity, string label, double value)
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
            var result = await collection.UpdateOneAsync(Builders<Cultivar>.Filter.Eq("_id", entity.id), Builders<Cultivar>.Update.Set("threshold", entity.threshold));
            return result.ModifiedCount > 0;
        }
    }
}
