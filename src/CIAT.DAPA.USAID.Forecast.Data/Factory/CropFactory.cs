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
    /// This class allow to get information about crop collection
    /// </summary>
    public class CropFactory: FactoryDB<Crop>
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public CropFactory(IMongoDatabase database): base(database, LogEntity.cp_crop)
        {

        }

        public async override Task<bool> updateAsync(Crop entity, Crop newEntity)
        {
            newEntity.track = entity.track;
            newEntity.track.updated = DateTime.Now;
            newEntity.setup = entity.setup.Count() == 0 ? new List<Setup>() : entity.setup;
            var result = await collection.ReplaceOneAsync(Builders<Crop>.Filter.Eq("_id", entity.id), newEntity);
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// Method that update the property setup of the crops
        /// </summary>
        /// <param name="entity">Crop with the new setup</param>
        /// <returns>True if the entity is updated, false otherwise</returns>
        public async Task<bool> updateSetupAsync(Crop entity)
        {
            var result = await collection.UpdateOneAsync(Builders<Crop>.Filter.Eq("_id", entity.id), Builders<Crop>.Update.Set("setup", entity.setup));
            return result.ModifiedCount > 0;
        }

        public async override Task<bool> deleteAsync(Crop entity)
        {
            var result = await collection.UpdateOneAsync(Builders<Crop>.Filter.Eq("_id", entity.id), Builders<Crop>.Update.Set("track.enable", false)
                                                                                               .Set("track.updated", DateTime.Now));
            return result.ModifiedCount > 0;
        }

        public async override Task<Crop> insertAsync(Crop entity)
        {
            entity.track = new Track() { enable = true, register = DateTime.Now, updated = DateTime.Now };
            entity.setup = new List<Setup>();
            await collection.InsertOneAsync(entity);
            return entity;
        }
    }
}
