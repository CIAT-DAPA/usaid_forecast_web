using CIAT.DAPA.USAID.Forecast.WebAPI.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAPI.Models.Entities
{
    public class RecommendationEntity
    {
        public string ws_id { get; set; }
        public string ws_name { get; set; }
        public string type { get; set; }
        public List<RecommendationKey> keys { get; set; }
        public string content { get; set; }
        public override string ToString()
        {
            string all_keys = "";
            foreach (var k in keys)
                all_keys += "{\"tag\":\"" + (k.tag ?? "") + "\",\"value\":\"" + (k.value ?? "") + "\",\"id\":\"" + (k.id ?? "") + "\"}";
            return ws_id + "," + ws_name + "," + type + ",\"" + content + "\"," + all_keys + "\n";
        }
    }
}
