using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.Data.Factory;
using CIAT.DAPA.USAID.Forecast.Data.Models;
using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Tools;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Tools
{
    /// <summary>
    /// This class keep a track over actions and events in the website
    /// </summary>
    public class Log
    {
        /// <summary>
        /// Get or set the path where the log file is located
        /// </summary>
        private string path { get; set; }
        /// <summary>
        /// Get or set the factory to save the log event in the database
        /// </summary>
        private LogAdministrativeFactory db { get; set; }

        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="path_log">Path log</param>
        /// <param name="logDB">Factory instance to save data in the database</param>
        public Log(string path_log, LogAdministrativeFactory factory)
        {
            path = path_log;
            db = factory;
        }

        /// <summary>
        /// Method that write a log event. It tries write the event in the database, but if it has a error, later tries to write the 
        /// event in a file
        /// </summary>
        /// <param name="content">Description of the event</param>
        /// <param name="entities">List of entities affected</param>
        /// <param name="e">Type event</param>
        /// <param name="user">User that start the event</param>
        public async void writeAsync(string content, List<LogEntity> entities, LogEvent e, string user)
        {
            var data = new CIAT.DAPA.USAID.Forecast.Data.Models.Log()
            {
                content = content,
                date = DateTime.Now,
                entities = entities,
                type_event = e,
                user = user
            };
            try
            {
                await db.insertAsync(new LogAdministrative() { data = data });
            }
            catch (Exception ex)
            {
                try
                {
                    File.AppendAllText(path + DateTime.Now.ToString("yyyyMMdd"), data.ToString() + "\n");
                }
                catch(Exception ex2)
                {
                    Console.WriteLine(ex.Message);
                }
                
            }
        }

    }
}
