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
    /// This class allow to get information about climatology collection
    /// </summary>
    public class ClimatologyFactory : FactoryDB<Climatology>
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public ClimatologyFactory(IMongoDatabase database): base(database, LogEntity.hs_climatology)
        {

        }
        
        public async override Task<bool> updateAsync(Climatology entity, Climatology newEntity)
        {
            throw new NotImplementedException();
        }

        public async override Task<bool> deleteAsync(Climatology entity)
        {
            throw new NotImplementedException();
        }

        public async override Task<Climatology> insertAsync(Climatology entity)
        {
            throw new NotImplementedException();
        }
    }
}
