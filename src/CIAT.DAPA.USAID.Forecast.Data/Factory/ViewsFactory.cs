using CIAT.DAPA.USAID.Forecast.Data.Database;
using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using CIAT.DAPA.USAID.Forecast.Data.Views;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Factory
{
    public class ViewsFactory
    {
        /// <summary>
        /// Get or set the mongo database
        /// </summary>
        protected IMongoDatabase db { get; private set; }

        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="database">Database connected</param>
        public ViewsFactory(IMongoDatabase database)
        {
            db = database;
        }
        
        public async Task<IEnumerable<LocationView>> listLocationVisibleAsync()
        {   
            var states = db.GetCollection<State>(Enum.GetName(typeof(LogEntity), LogEntity.lc_state)).AsQueryable().Where(f => f.track.enable).ToList();
            var municipalities = db.GetCollection<Municipality>(Enum.GetName(typeof(LogEntity), LogEntity.lc_municipality)).AsQueryable().Where(f => f.track.enable && f.visible).ToList();
            var weatherstations = db.GetCollection<WeatherStation>(Enum.GetName(typeof(LogEntity), LogEntity.lc_weather_station)).AsQueryable().Where(f => f.track.enable && f.visible).ToList();
            var query = from s in states
                        join m in municipalities on s.id equals m.state
                        join w in weatherstations on m.id equals w.municipality
                        group new { s, m, w } by m.state into joined
                        select joined.Select(p => new LocationView()
                        {
                            st_id = p.s.id.ToString(),
                            st_name = p.s.name,
                            st_country = p.s.country.name,
                            mn_id = p.m.id.ToString(),
                            mn_name = p.m.name,
                            ws_id = p.w.id.ToString(),
                            ws_name = p.w.name,
                            ws_origin = p.w.origin,
                            ws_ext = p.w.ext_id,
                            ws_lat = p.w.latitude,
                            ws_lon = p.w.longitude
                        });
            return query.FirstOrDefault();
        }
    }
}
