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
    /// This class allow to get information about states collection
    /// </summary>
    public class CountryFactory : FactoryDB<Country>
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public CountryFactory(IMongoDatabase database) : base(database, LogEntity.lc_country)
        {

        }

        public async override Task<bool> updateAsync(Country entity, Country newEntity)
        {
            newEntity.track = entity.track;
            newEntity.track.updated = DateTime.Now;
            var result = await collection.ReplaceOneAsync(Builders<Country>.Filter.Eq("_id", entity.id), newEntity);
            return result.ModifiedCount > 0;
        }

        public async override Task<bool> deleteAsync(Country entity)
        {
            var result = await collection.UpdateOneAsync(Builders<Country>.Filter.Eq("_id", entity.id), Builders<Country>.Update.Set("track.enable", false)
                                                                                               .Set("track.updated", DateTime.Now));
            return result.ModifiedCount > 0;
        }

        public async override Task<Country> insertAsync(Country entity)
        {
            entity.track = new Track() { enable = true, register = DateTime.Now, updated = DateTime.Now };            
            await collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task<List<Country>> listAllAsync()
        {
            return await collection.Find("{}").ToListAsync<Country>();
        }

    }
}
