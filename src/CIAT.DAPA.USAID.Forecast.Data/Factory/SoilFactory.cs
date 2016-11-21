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
    /// This class allow to get information about soils collection
    /// </summary>
    public class SoilFactory: FactoryDB<Soil>
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public SoilFactory(IMongoDatabase database): base(database, LogEntity.cp_soil)
        {

        }

        public async override Task<bool> updateAsync(Soil entity, Soil newEntity)
        {
            var result = await collection.ReplaceOneAsync(Builders<Soil>.Filter.Eq("_id", entity.id), newEntity);
            return true;
        }

        public async override Task<bool> deleteAsync(Soil entity)
        {
            await collection.UpdateOneAsync(Builders<Soil>.Filter.Eq("_id", entity.id), Builders<Soil>.Update.Set("track.enable", false)
                                                                                               .Set("track.updated", DateTime.Now));
            return true;
        }
    }
}
