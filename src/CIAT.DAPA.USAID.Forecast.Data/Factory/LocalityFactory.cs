using CIAT.DAPA.USAID.Forecast.Data.Factory;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Factory
{
    public class LocalityFactory : FactoryDB<Locality>
    {        
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public LocalityFactory(IMongoDatabase database): base(database)
        {

        }
    }
}
