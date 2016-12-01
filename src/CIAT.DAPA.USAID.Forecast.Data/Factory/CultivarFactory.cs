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

        public async override Task<List<Cultivar>> listEnableAsync()
        {
            var builder = Builders<Cultivar>.Filter;
            var filter = builder.Eq("track.enable", true);
            return await collection.Find(filter).SortBy(p=> p.order).ToListAsync<Cultivar>();
        }
    }
}
