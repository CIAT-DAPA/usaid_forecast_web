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
    public class RecommendationFactory : FactoryDB<Recommendation>
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public RecommendationFactory(IMongoDatabase database): base(database, LogEntity.cp_recommendation)
        {
            var indexKeys = Builders<Recommendation>.IndexKeys.Combine(
                            Builders<Recommendation>.IndexKeys.Ascending(x => x.country),
                            Builders<Recommendation>.IndexKeys.Ascending(x => x.type_enum),
                            Builders<Recommendation>.IndexKeys.Ascending(x => x.type_resp),
                            Builders<Recommendation>.IndexKeys.Ascending(x => x.lang));
            var indexOptions = new CreateIndexOptions { Unique = false }; // if you want the index is unique
            var indexModel = new CreateIndexModel<Recommendation>(indexKeys, indexOptions);
            collection.Indexes.CreateOne(indexModel);
        }

        public async override Task<bool> updateAsync(Recommendation entity, Recommendation newEntity)
        {
            var result = await collection.ReplaceOneAsync(Builders<Recommendation>.Filter.Eq("_id", entity.id), newEntity);
            return result.ModifiedCount > 0;
        }

        public async virtual Task<List<Recommendation>> listAsync()
        {
            return await collection.Find(_ => true).ToListAsync();
        }

        public async override Task<bool> deleteAsync(Recommendation entity)
        {
            throw new NotImplementedException();
        }

        public async override Task<Recommendation> insertAsync(Recommendation entity)
        {
            await collection.InsertOneAsync(entity);
            return entity;
        }

        /// <summary>
        /// Method that return all records about phenological phases of the forecast
        /// by forecast
        /// </summary>
        /// <param name="forecast">Id Forecast</param>
        /// <returns>List of the Forecast Climate</returns>
        public async Task<IEnumerable<Recommendation>> byCountryAsync(ObjectId country)
        {
            var builder = Builders<Recommendation>.Filter;
            var filter = builder.Eq("country", country);
            return await collection.Find(filter).ToListAsync<Recommendation>();
        }

        /// <summary>
        /// Method that return all records about phenological phases of the forecast
        /// by forecast
        /// </summary>
        /// <param name="forecast">Id Forecast</param>
        /// <param name="ws">ID weather station array</param>
        /// <returns>List of the Forecast Climate</returns>

        public async Task<IEnumerable<Recommendation>> byIndexAsync(ObjectId country, string type_enum, string type_resp, RecommendationLang lang)
        {
            var builder = Builders<Recommendation>.Filter;
            var filter = builder.And(builder.Eq(x => x.country, country), builder.Eq(x => x.type_enum, type_enum), builder.Eq(x => x.type_resp, type_resp), builder.Eq(x => x.lang, lang.ToString()));
            var query = await collection.Find(filter).ToListAsync<Recommendation>();
            return query;
        }
    }
}
