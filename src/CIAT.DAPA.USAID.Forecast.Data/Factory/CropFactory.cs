using CIAT.DAPA.USAID.Forecast.Data.Database;
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
    public class CropFactory : FactoryDB<Crop>
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public CropFactory(IMongoDatabase database) : base(database, LogEntity.cp_crop)
        {

        }

        public async override Task<bool> updateAsync(Crop entity, Crop newEntity)
        {
            newEntity.track = entity.track;
            newEntity.track.updated = DateTime.Now;
            //newEntity.setup = entity.setup.Count() == 0 ? new List<Setup>() : entity.setup;
            var result = await collection.ReplaceOneAsync(Builders<Crop>.Filter.Eq("_id", entity.id), newEntity);
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// Method that add a new setup to a crop
        /// </summary>
        /// <param name="entity">Crop with the new setup</param>
        /// <param name="setup">New setup to add to the crop</param>
        /// <returns>True if the entity is updated, false otherwise</returns>
        //public async Task<bool> addSetupAsync(Crop entity, Setup setup)
        //{
        //    setup.track = new Track() { enable = true, register = DateTime.Now, updated = DateTime.Now };
        //    List<Setup> allSetups = entity.setup.ToList();
        //    allSetups.Add(setup);
        //    entity.setup = allSetups;
        //    var result = await collection.UpdateOneAsync(Builders<Crop>.Filter.Eq("_id", entity.id), Builders<Crop>.Update.Set("setup", entity.setup));
        //    return result.ModifiedCount > 0;
        //}

        /// <summary>
        /// Method that delete a setup entity from a crop
        /// </summary>
        /// <param name="entity">Crop to delete the setup configuration</param>
        /// <param name="ws">Id of the weather station</param>
        /// <param name="cu">Id of the cultivar</param>
        /// <param name="so">Id of the soil</param>
        /// <param name="days">Days</param>
        /// <returns>True if the entity is updated, false otherwise</returns>
        //public async Task<bool> deleteSetupAsync(Crop entity, string ws, string cu, string so, int days)
        //{
        //    List<Setup> allSetups = new List<Setup>();
        //    // This cicle search the setup to delete
        //    foreach (var s in entity.setup)
        //    {
        //        // If the setup is found, it will update
        //        if (s.weather_station == ForecastDB.parseId(ws) && s.cultivar == ForecastDB.parseId(cu) && s.soil == ForecastDB.parseId(so) && s.days == days)
        //        {
        //            s.track.updated = DateTime.Now;
        //            s.track.enable = false;
        //        }
        //        allSetups.Add(s);
        //    }
        //    entity.setup = allSetups;
        //    var result = await collection.UpdateOneAsync(Builders<Crop>.Filter.Eq("_id", entity.id), Builders<Crop>.Update.Set("setup", entity.setup));
        //    return result.ModifiedCount > 0;
        //}

        public async override Task<bool> deleteAsync(Crop entity)
        {
            var result = await collection.UpdateOneAsync(Builders<Crop>.Filter.Eq("_id", entity.id), Builders<Crop>.Update.Set("track.enable", false)
                                                                                               .Set("track.updated", DateTime.Now));
            return result.ModifiedCount > 0;
        }

        public async override Task<Crop> insertAsync(Crop entity)
        {
            entity.track = new Track() { enable = true, register = DateTime.Now, updated = DateTime.Now };
            //entity.setup = new List<Setup>();
            await collection.InsertOneAsync(entity);
            return entity;
        }

        /// <summary>
        /// Method that add a new config to crop
        /// </summary>
        /// <param name="entity">Crop to add new crop config</param>
        /// <param name="crop_config">New crop config to add to the crop</param>
        /// <returns>True if the entity is updated, false otherwise</returns>
        public async Task<bool> addCropConfigAsync(Crop entity, CropConfig crop_config)
        {
            List<CropConfig> allCropConfigs = new List<CropConfig>();
            if (entity.crop_config != null)
            {
                allCropConfigs = entity.crop_config.ToList();
            }
            allCropConfigs.Add(crop_config);
            entity.crop_config = allCropConfigs;
            var result = await collection.UpdateOneAsync(Builders<Crop>.Filter.Eq("_id", entity.id), Builders<Crop>.Update.Set("crop_config", entity.crop_config));
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// Method that delete a crop config entity from a crop
        /// </summary>
        /// <param name="entity">Crop to delete the crop config</param>
        /// <param name="label">Name of level</param>
        /// <param name="min">Minimum</param>
        /// <param name="max">Maximum</param>
        /// <returns>True if the entity is updated, false otherwise</returns>
        public async Task<bool> deleteCropConfigAsync(Crop entity, string label, double min, double max, string type)
        {
            List<CropConfig> allCropConfigs = new List<CropConfig>();
            // This cicle search the range to delete
            foreach (var r in entity.crop_config)
            {
                // If the setup is found, it will update
                if (!( r.label.Equals(label) && r.min == min && r.max == max && r.type == type))
                    allCropConfigs.Add(r);
            }
            entity.crop_config = allCropConfigs;
            var result = await collection.UpdateOneAsync(Builders<Crop>.Filter.Eq("_id", entity.id), Builders<Crop>.Update.Set("crop_config", entity.crop_config));
            return result.ModifiedCount > 0;
        }
    }
}
