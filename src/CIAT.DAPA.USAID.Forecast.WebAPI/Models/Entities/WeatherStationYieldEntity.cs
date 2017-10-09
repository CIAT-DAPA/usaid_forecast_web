using CIAT.DAPA.USAID.Forecast.Data.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAPI.Models.Entities
{
    public class WeatherStationYieldEntity
    {
        public ObjectId ws { get; set; }
        public List<YieldCropEntity> yield { get; set; }
    }
}
