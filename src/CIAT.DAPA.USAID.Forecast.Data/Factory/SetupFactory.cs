using CIAT.DAPA.USAID.Forecast.Data.Database;
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
    /// This class allow to get information about setup collection
    /// </summary>
    public class SetupFactory : FactoryDB<Setup>
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public SetupFactory(IMongoDatabase database) : base(database, LogEntity.cp_setup)
        {

        }

        public async override Task<bool> deleteAsync(Setup entity)
        {
            var result = await collection.UpdateOneAsync(Builders<Setup>.Filter.Eq("_id", entity.id), Builders<Setup>.Update.Set("track.enable", false)
                                                                                   .Set("track.updated", DateTime.Now));
            return result.ModifiedCount > 0;
        }

        public async override Task<Setup> insertAsync(Setup entity)
        {
            entity.track = new Track() { enable = true, register = DateTime.Now, updated = DateTime.Now };
            await collection.InsertOneAsync(entity);
            return entity;
        }

        public async override Task<bool> updateAsync(Setup entity, Setup newEntity)
        {
            newEntity.track = entity.track;
            newEntity.track.updated = DateTime.Now;
            var result = await collection.ReplaceOneAsync(Builders<Setup>.Filter.Eq("_id", entity.id), newEntity);
            return result.ModifiedCount > 0;
        }

        public async virtual Task<List<Setup>> listEnableDescAsync()
        {
            var builder = Builders<Setup>.Filter;
            var filter = builder.Eq("track.enable", true);
            return await collection.Find(filter).SortByDescending(p => p.track.register).ToListAsync<Setup>();
        }
    }
}
