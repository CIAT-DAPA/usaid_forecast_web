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
    /// This class allow to get information about crop collection
    /// </summary>
    public class CropFactory: FactoryDB<Crop>
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public CropFactory(IMongoDatabase database): base(database, LogEntity.cp_crop)
        {

        }

        public async override Task<bool> updateAsync(Crop entity, Crop newEntity)
        {
            var result = await collection.ReplaceOneAsync(Builders<Crop>.Filter.Eq("_id", entity.id), newEntity);
            return true;
        }

        public async override Task<bool> deleteAsync(Crop entity)
        {
            await collection.UpdateOneAsync(Builders<Crop>.Filter.Eq("_id", entity.id), Builders<Crop>.Update.Set("track.enable", false)
                                                                                               .Set("track.updated", DateTime.Now));
            return true;
        }
    }
}
