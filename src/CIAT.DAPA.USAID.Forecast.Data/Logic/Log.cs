using CIAT.DAPA.USAID.Forecast.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    public partial class Log
    {
        /// <summary>
        /// Method that return a string representation of the log data 
        /// </summary>
        /// <returns>String with the content of the log</returns>
        public override string ToString()
        {
            string list_entities = string.Empty;
            foreach(var e in entities)
                list_entities += Enum.GetName(typeof(LogEntity), e) + "-";
            return date.ToString("yyyy-MM-dd HH:mm:ss") + "|" + Enum.GetName(typeof(LogEvent), type_event) + "|" + user + "|" + list_entities + "|" + content ;
        }
    }
}
