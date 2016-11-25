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
    /// This class allow to get information about service log collection
    /// </summary>
    public class LogServiceFactory: FactoryDB<LogService>
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public LogServiceFactory(IMongoDatabase database): base(database, LogEntity.log_service)
        {

        }

        public async override Task<bool> updateAsync(LogService entity, LogService newEntity)
        {
            throw new NotImplementedException();
        }

        public async override Task<bool> deleteAsync(LogService entity)
        {
            throw new NotImplementedException();
        }

        public async override Task<LogService> insertAsync(LogService entity)
        {
            await collection.InsertOneAsync(entity);
            return entity;
        }
    }
}
