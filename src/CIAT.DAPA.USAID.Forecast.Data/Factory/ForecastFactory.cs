using CIAT.DAPA.USAID.Forecast.Data.Enums;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CIAT.DAPA.USAID.Forecast.Data.Models;

namespace CIAT.DAPA.USAID.Forecast.Data.Factory
{
    /// <summary>
    /// This class allow to get information about forecast collection
    /// </summary>
    public class ForecastFactory : FactoryDB<CIAT.DAPA.USAID.Forecast.Data.Models.Forecast>
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public ForecastFactory(IMongoDatabase database) : base(database, LogEntity.fs_forecast)
        {

        }

        public async override Task<bool> updateAsync(Models.Forecast entity, Models.Forecast newEntity)
        {
            newEntity.track = entity.track;
            newEntity.track.updated = DateTime.Now;
            var result = await collection.ReplaceOneAsync(Builders<Models.Forecast>.Filter.Eq("_id", entity.id), newEntity);
            return result.ModifiedCount > 0;
        }

        public async override Task<bool> deleteAsync(Models.Forecast entity)
        {
            var result = await collection.UpdateOneAsync(Builders<Models.Forecast>.Filter.Eq("_id", entity.id),
                                                        Builders<Models.Forecast>.Update.Set("track.enable", false)
                                                        .Set("track.updated", DateTime.Now));
            return result.ModifiedCount > 0;
        }

        public async override Task<Models.Forecast> insertAsync(Models.Forecast entity)
        {
            entity.track = new Track() { enable = true, register = DateTime.Now, updated = DateTime.Now };
            await collection.InsertOneAsync(entity);
            return entity;
        }

        /// <summary>
        /// Method that return last forecast inserted in the database
        /// </summary>
        /// <returns>Forecast</returns>
        public async Task<Models.Forecast> getLatestAsync(bool skip = false)
        {
            var builder = Builders<Models.Forecast>.Filter;
            var filter = builder.Eq("track.enable", true);

            Models.Forecast forecast = skip ?
                await collection.Find(filter).SortByDescending(p => p.id).Skip(1).FirstOrDefaultAsync<Models.Forecast>() :
                await collection.Find(filter).SortByDescending(p => p.id).FirstOrDefaultAsync<Models.Forecast>();

            return forecast;
        }

        /// <summary>
        /// Method that return last forecast inserted in the database
        /// </summary>
        /// <returns>Forecast</returns>
        public async Task<List<Models.Forecast>> getExceedanceAsync()
        {
            var builder = Builders<Models.Forecast>.Filter;
            var filter = builder.Eq("track.enable", true);
            return await collection.Find(filter).SortByDescending(p => p.id).Limit(6).ToListAsync<Models.Forecast>();
        }

        /// <summary>
        /// Method that return all forecast inserted for years
        /// </summary>
        /// <returns>List of Forecast</returns>
        public async Task<List<Models.Forecast>> getByYearstAsync(int year)
        {
            DateTime init = new DateTime(year, 1, 1);
            DateTime end = new DateTime(year, 12, 31);
            var query = from fs in collection.AsQueryable()
                        where fs.track.enable &&  fs.start >= init && fs.start <= end
                        select fs;
            return query.ToList();
        }

    }
}
