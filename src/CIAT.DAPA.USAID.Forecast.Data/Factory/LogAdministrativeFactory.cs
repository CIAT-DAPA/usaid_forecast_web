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
    /// This class allow to get information about administrative log collection
    /// </summary>
    public class LogAdministrativeFactory: FactoryDB<LogAdministrative>
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public LogAdministrativeFactory(IMongoDatabase database): base(database, LogEntity.log_administrative)
        {

        }

        public async override Task<bool> updateAsync(LogAdministrative entity, LogAdministrative newEntity)
        {
            throw new NotImplementedException();
        }

        public async override Task<bool> deleteAsync(LogAdministrative entity)
        {
            throw new NotImplementedException();
        }

        public async override Task<LogAdministrative> insertAsync(LogAdministrative entity)
        {            
            await collection.InsertOneAsync(entity);
            return entity;
        }
    }
}
