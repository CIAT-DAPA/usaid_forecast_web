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
    /// This class allow to get information about historical yield collection
    /// </summary>
    public class HistoricalYieldFactory : FactoryDB<HistoricalYield>
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public HistoricalYieldFactory(IMongoDatabase database) : base(database, LogEntity.hs_historical_yield)
        {

        }

        public async override Task<bool> updateAsync(HistoricalYield entity, HistoricalYield newEntity)
        {
            var result = await collection.ReplaceOneAsync(Builders<HistoricalYield>.Filter.Eq("_id", entity.id), newEntity);
            return result.ModifiedCount > 0;
        }

        public async override Task<bool> deleteAsync(HistoricalYield entity)
        {
            throw new NotImplementedException();
        }

        public async override Task<HistoricalYield> insertAsync(HistoricalYield entity)
        {
            entity.date = DateTime.Now;
            await collection.InsertOneAsync(entity);
            return entity;
        }

        /// <summary>
        /// Method that return all registers in the database 
        /// by the weather station and source
        /// </summary>
        /// <param name="ws">ID weather station</param>
        /// <param name="source">Name of source</param>
        /// <returns>List of the historical yield data</returns>
        public async virtual Task<HistoricalYield> byWeatherStationSourceAsync(ObjectId ws, string source)
        {
            // Filter all entities available.
            var query = from hc in collection.AsQueryable()
                        where ws == hc.weather_station && hc.source.Equals(source)
                        select hc;
            return query.FirstOrDefault();
        }

        /// <summary>
        /// Method that return all registers in the database 
        /// by the weather stations and years
        /// </summary>
        /// <param name="ws">Array of the weather stations ids</param>
        /// <param name="years">Array of the years to search information</param>
        /// <returns>List of the historical yield data</returns>
        public async virtual Task<List<HistoricalYield>> byWeatherStationsYearsAsync(ObjectId[] ws, List<int> years)
        {
            List<HistoricalYield> answer = new List<HistoricalYield>();
            // Filter by weather station.
            var query = collection.AsQueryable().Where(p => ws.Contains(p.weather_station)).ToList();
            // Get only the years needed
            foreach (var hy in query)
                answer.Add(new HistoricalYield()
                {
                    id = hy.id,
                    source = hy.source,
                    weather_station = hy.weather_station,
                    cultivar = hy.cultivar,
                    soil = hy.soil,
                    yield = hy.yield.Where(p2 => years.Contains(p2.start.Year)).OrderBy(p => p.start).Select(p => new YieldCrop()
                    {                        
                        data = p.data,
                        start = DateTime.SpecifyKind(p.start, DateTimeKind.Utc),
                        end = DateTime.SpecifyKind(p.end, DateTimeKind.Utc)

                    })
                });

            return answer;
        }

        /// <summary>
        /// Method that return all year that have information in the database 
        /// by the weather stations
        /// </summary>
        /// <param name="ws">Array of the weather stations ids</param>
        /// <returns>List of years with information</returns>
        public async virtual Task<List<int>> getYearsAvailableAsync(ObjectId[] ws)
        {
            // Filter all entities available.            
            List<int> years = new List<int>();
            var yields = collection.Find(p => ws.Contains(p.weather_station)).Project(x => x.yield).ToListAsync().Result;
            foreach (var y in yields)
                years.AddRange(y.Select(p => p.start.Year));
            return years.Distinct().OrderBy(p => p).ToList();
        }
    }
}
