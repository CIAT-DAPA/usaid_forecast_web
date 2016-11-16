using CIAT.DAPA.USAID.Forecast.Data.Enums;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Factory
{
    /// <summary>
    /// This class allow to get information about forecast collection
    /// </summary>
    public class ForecastFactory: FactoryDB<CIAT.DAPA.USAID.Forecast.Data.Models.Forecast>
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public ForecastFactory(IMongoDatabase database): base(database, LogEntity.fs_forecast)
        {

        }
    }
}
