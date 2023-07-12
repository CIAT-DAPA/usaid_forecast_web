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
    /// This class allow to get information about phenological phases forecast collection
    /// </summary>
    public class UrlFactory : FactoryDB<Url>
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public UrlFactory(IMongoDatabase database): base(database, LogEntity.cp_url)
        {
            var indexKeys = Builders<Url>.IndexKeys.Combine(
                            Builders<Url>.IndexKeys.Ascending(x => x.country),
                            Builders<Url>.IndexKeys.Ascending(x => x.type));
            var indexOptions = new CreateIndexOptions { Unique = true }; // if you want the index is unique
            var indexModel = new CreateIndexModel<Url>(indexKeys, indexOptions);
            collection.Indexes.CreateOne(indexModel);
        }

        public async override Task<bool> updateAsync(Url entity, Url newEntity)
        {
            newEntity.track = entity.track;
            newEntity.track.updated = DateTime.Now;
            var result = await collection.ReplaceOneAsync(Builders<Url>.Filter.Eq("_id", entity.id), newEntity);
            return result.ModifiedCount > 0;
        }

        public async virtual Task<List<Url>> listAsync()
        {
            return await collection.Find(_ => true).ToListAsync();
        }

        public async override Task<bool> deleteAsync(Url entity)
        {

            FilterDefinition<Url> filter = Builders<Url>.Filter.Eq("_id", entity.id);
            var result = collection.DeleteOne(filter);
            return result.DeletedCount > 0;
        }

        public async override Task<Url> insertAsync(Url entity)
        {
            entity.track = new Track() { enable = true, register = DateTime.Now, updated = DateTime.Now };
            await collection.InsertOneAsync(entity);
            return entity;
        }

        /// <summary>
        /// Method that return all records about phenological phases of the forecast
        /// by forecast
        /// </summary>
        /// <param name="forecast">Id Forecast</param>
        /// <returns>List of the Forecast Climate</returns>
        public async Task<IEnumerable<Url>> byCountryAsync(ObjectId country)
        {
            var builder = Builders<Url>.Filter;
            var filter = builder.Eq("country", country);
            return await collection.Find(filter).ToListAsync<Url>();
        }

        /// <summary>
        /// Method that return all records about phenological phases of the forecast
        /// by forecast
        /// </summary>
        /// <param name="forecast">Id Forecast</param>
        /// <param name="ws">ID weather station array</param>
        /// <returns>List of the Forecast Climate</returns>

        public async Task<IEnumerable<Url>> byIndexAsync(ObjectId country, string type)
        {
            var builder = Builders<Url>.Filter;
            var filter = builder.And(builder.Eq(x => x.country, country), builder.Eq(x => x.type, (UrlTypes)Enum.Parse(typeof(UrlTypes), type)));
            var query = await collection.Find(filter).ToListAsync<Url>();
            return query;
        }
    }
}
