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
    /// This class allow to get information about historical daily climatic data collection
    /// </summary>
    public class HistoricalDailyDataFactory : FactoryDB<WeatherStationDailyData>
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public HistoricalDailyDataFactory(IMongoDatabase database) : base(database, LogEntity.hs_historical_daily_data)
        {
            // Index 1: weather_station and year
            var indexKeys_year = Builders<WeatherStationDailyData>.IndexKeys.Combine(
                Builders<WeatherStationDailyData>.IndexKeys.Ascending(x => x.weather_station),
                Builders<WeatherStationDailyData>.IndexKeys.Ascending(x => x.year));
            var indexOptions_year = new CreateIndexOptions { Unique = false };
            var indexModel_year = new CreateIndexModel<WeatherStationDailyData>(indexKeys_year, indexOptions_year);
            collection.Indexes.CreateOne(indexModel_year);

            // Index 2: weather_station and month
            var indexKeys_month = Builders<WeatherStationDailyData>.IndexKeys.Combine(
                Builders<WeatherStationDailyData>.IndexKeys.Ascending(x => x.weather_station),
                Builders<WeatherStationDailyData>.IndexKeys.Ascending(x => x.month));
            var indexOptions_month = new CreateIndexOptions { Unique = false };
            var indexModel_month = new CreateIndexModel<WeatherStationDailyData>(indexKeys_month, indexOptions_month);
            collection.Indexes.CreateOne(indexModel_month);

            // Index 3: weather_station, year and month
            var indexKeys = Builders<WeatherStationDailyData>.IndexKeys.Combine(
                            Builders<WeatherStationDailyData>.IndexKeys.Ascending(x => x.weather_station),
                            Builders<WeatherStationDailyData>.IndexKeys.Ascending(x => x.year),
                            Builders<WeatherStationDailyData>.IndexKeys.Ascending(x => x.month));
            var indexOptions = new CreateIndexOptions { Unique = true };
            var indexModel = new CreateIndexModel<WeatherStationDailyData>(indexKeys, indexOptions);
            collection.Indexes.CreateOne(indexModel);
        }

        public async override Task<bool> updateAsync(WeatherStationDailyData entity, WeatherStationDailyData newEntity)
        {
            var result = await collection.ReplaceOneAsync(Builders<WeatherStationDailyData>.Filter.Eq("_id", entity.id), newEntity);
            return result.ModifiedCount > 0;
        }

        public async override Task<bool> deleteAsync(WeatherStationDailyData entity)
        {
            throw new NotImplementedException();
        }

        public async override Task<WeatherStationDailyData> insertAsync(WeatherStationDailyData entity)
        {
            await collection.InsertOneAsync(entity);
            return entity;
        }

        /// <summary>
        /// Method that return climactic daily data per year in the database 
        /// by the weather stations
        /// </summary>
        ///<param name = "ws" > weather station id who contain climatic data</param>
        /// <param name="year">year of the climatic data</param>
        /// <returns>List of the historical daily climatic data</returns>
        public async virtual Task<List<WeatherStationDailyData>> byYearWeatherStationAsync(int year, ObjectId ws)
        {

            var builder = Builders<WeatherStationDailyData>.Filter;
            var filter = builder.And(builder.Eq(x => x.weather_station, ws), builder.Eq(x => x.year, year));
            var query = await collection.Find(filter).ToListAsync<WeatherStationDailyData>();
            return query;

        }

        /// <summary>
        /// Method that return climactic daily data per month in the database 
        /// by the weather stations
        /// </summary>
        ///<param name = "ws" > weather station id who contain climatic data</param>
        /// <param name="month">month of the climatic dat</param>
        /// <returns>List of the historical daily climatic data</returns>
        public async virtual Task<List<WeatherStationDailyData>> byMonthWeatherStationsAsync(int month, ObjectId ws)
        {
            var builder = Builders<WeatherStationDailyData>.Filter;
            var filter = builder.And(builder.Eq(x => x.weather_station, ws), builder.Eq(x => x.month, month));
            var query = await collection.Find(filter).ToListAsync<WeatherStationDailyData>();
            return query;
        }

        /// <summary>
        /// Method that return climactic daily data per year and month in the database 
        /// by the weather stations
        /// </summary>
        /// <param name="ws">weather station id who contain climatic data</param>
        /// <param name="year">year of the climatic data</param>
        /// <param name="month">month of the climatic dat</param>
        /// <returns>List of the historical daily climatic data</returns>
        public async virtual Task<List<WeatherStationDailyData>> DailyDataAsync(int year, int month, ObjectId ws)
        {
            var builder = Builders<WeatherStationDailyData>.Filter;
            var filter = builder.And(builder.Eq(x => x.weather_station, ws), builder.Eq(x => x.year, year), builder.Eq(x => x.month, month));
            var query = await collection.Find(filter).ToListAsync<WeatherStationDailyData>();
            return query;
        }
    }
}
