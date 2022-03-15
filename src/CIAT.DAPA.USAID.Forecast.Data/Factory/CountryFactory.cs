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
    /// This class allow to get information about states collection
    /// </summary>
    public class CountryFactory : FactoryDB<Country>
    {
        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected to mongo</param>
        public CountryFactory(IMongoDatabase database) : base(database, LogEntity.lc_country)
        {

        }

        public async override Task<bool> updateAsync(Country entity, Country newEntity)
        {
            newEntity.track = entity.track;
            newEntity.track.updated = DateTime.Now;
            var result = await collection.ReplaceOneAsync(Builders<Country>.Filter.Eq("_id", entity.id), newEntity);
            return result.ModifiedCount > 0;
        }

        public async override Task<bool> deleteAsync(Country entity)
        {
            var result = await collection.UpdateOneAsync(Builders<Country>.Filter.Eq("_id", entity.id), Builders<Country>.Update.Set("track.enable", false)
                                                                                               .Set("track.updated", DateTime.Now));
            return result.ModifiedCount > 0;
        }

        public async override Task<Country> insertAsync(Country entity)
        {
            entity.track = new Track() { enable = true, register = DateTime.Now, updated = DateTime.Now };            
            await collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task<List<Country>> listAllAsync()
        {
            return await collection.Find("{}").ToListAsync<Country>();
        }

        public async Task<bool> addConfigurationPyCpt(Country country, ConfigurationPyCPT conf) 
        {
            conf.track = new Track() { enable = true, register = DateTime.Now, updated = DateTime.Now };
            List<ConfigurationPyCPT> allConf = country.conf_pycpt.ToList();
            allConf.Add(conf);
            country.conf_pycpt = allConf;
            var result = await collection.UpdateOneAsync(Builders<Country>.Filter.Eq("_id", country.id),
                Builders<Country>.Update.Set("conf_pycpt", country.conf_pycpt));
            return result.ModifiedCount > 0;
        }

        public async Task<bool> deleteConfigurationPyCPTAsync(Country entity, Obs obs, Mos mos, bool station, Predictand predictand, Predictors predictors, int tini, int tend, int xmodes_min, int xmodes_max, int ymodes_min, int ymodes_max, int ccamodes_min, int ccamodes_max, bool force_download, bool single_models, bool forecast_anomaly, bool forecast_spi, int confidence_level, int ind_exec, DateTime register)
        {
            List<ConfigurationPyCPT> allConf = new List<ConfigurationPyCPT>();
            foreach (var c in entity.conf_pycpt)
            {
                if (c.obs == obs && c.mos == mos && c.station == station && c.predictand == predictand && c.predictors == predictors && c.tini == tini && c.tend == tend && c.xmodes_min == xmodes_min && c.xmodes_max == xmodes_max && c.ymodes_min == ymodes_min && c.ymodes_max == ymodes_max && c.ccamodes_min == ccamodes_min && c.ccamodes_max == ccamodes_max && c.force_download == force_download && c.single_models == single_models && c.forecast_anomaly == forecast_anomaly && c.forecast_spi == forecast_spi && c.confidence_level == confidence_level && c.ind_exec == ind_exec && c.track.register.ToString("yyyy-MM-dd HH:mm:ss") == register.ToString("yyyy-MM-dd HH:mm:ss"))
                {
                    c.track.updated = DateTime.Now;
                    c.track.enable = false;
                }
                allConf.Add(c);
            }
            entity.conf_pycpt = allConf;
            var result = await collection.UpdateOneAsync(Builders<Country>.Filter.Eq("_id", entity.id),
                Builders<Country>.Update.Set("conf_pycpt", entity.conf_pycpt));
            return result.ModifiedCount > 0;
        }
    }
}
