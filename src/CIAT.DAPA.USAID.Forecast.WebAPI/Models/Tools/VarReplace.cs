using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.Data.Factory;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAPI.Models.Tools
{
    /// <summary>
    /// This class keep a track over actions and events in the web API
    /// </summary>
    public class VarReplace
    {
        /// <summary>
        /// Get or set the path where the log file is located
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Get or set the factory to save the log event in the database
        /// </summary>
        public string value { get; set; }

        public VarReplace(string new_name, string new_value)
        {
            name = new_name;
            value = new_value;
        }

        public static string createNewMsg(List<VarReplace> replaces, string msg)
        {
            string new_msg = msg;

            foreach (VarReplace replace in replaces)
            {
                if (new_msg.Contains($"{{{replace.name}}}"))
                {
                    new_msg = new_msg.Replace($"{{{replace.name}}}", replace.value);
                }
            }

            new_msg = new_msg.Replace(@"{[^}]+}", "");


            return new_msg;
        }

    }
}
