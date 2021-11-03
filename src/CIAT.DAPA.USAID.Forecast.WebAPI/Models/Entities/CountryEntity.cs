using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAPI.Models.Entities
{
    public class CountryEntity
    {
        public string id { get; set; }
        public string iso2 { get; set; }
        public string name { get; set; }
    }
}
