using CIAT.DAPA.USAID.Forecast.Data.Database;
using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Tools;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Controllers
{
    public class WebAdminBaseController : Controller
    {
        protected IHostingEnvironment hostingEnvironment { get; set; }
        /// <summary>
        /// Get or set object to connect with database
        /// </summary>
        protected ForecastDB db { get; set; }
        /// <summary>
        /// Get or set object to write the events occurring on the website 
        /// </summary>
        protected Log log { get; set; }
        /// <summary>
        /// List of the entities affected
        /// </summary>
        protected List<LogEntity> entities { get; set; }
        /// <summary>
        /// Current user
        /// </summary>
        protected string user { get; set; }
        /// <summary>
        /// Path where the imports files are located
        /// </summary>
        protected string importPath { get; set; }
        /// <summary>
        /// Path where the configuration files are located
        /// </summary>
        protected string configurationPath { get; set; }

        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Settings options</param>
        /// <param name="entity">List of entities affected</param>
        /// <param name="hostingEnvironment">Host Enviroment</param>
        public WebAdminBaseController(IOptions<Settings> settings, LogEntity entity, IHostingEnvironment environment) : base()
        {
            entities = new List<LogEntity>() { entity };
            user = "test";
            hostingEnvironment = environment;
            db = new ForecastDB(settings.Value.ConnectionString, settings.Value.Database);
            log = new Log(hostingEnvironment.ContentRootPath + settings.Value.LogPath, db.logAdministrative);
            importPath = hostingEnvironment.ContentRootPath + settings.Value.ImportPath;
            configurationPath = hostingEnvironment.ContentRootPath + settings.Value.ConfigurationPath;
        }

        /// <summary>
        /// Method that write event in the log
        /// </summary>
        /// <param name="content">Description event</param>
        /// <param name="e">Type event</param>
        /// <param name="entities_affected">List of the entities affected</param>
        public void writeEvent(string content, LogEvent e, List<LogEntity> entities_affected)
        {
            log.writeAsync(content, entities_affected, e, user);
        }

        /// <summary>
        /// Method that write event in the log
        /// </summary>
        /// <param name="content">Description event</param>
        /// <param name="e">Type event</param>
        public void writeEvent(string content, LogEvent e)
        {
            writeEvent(content, e, entities);
        }

        /// <summary>
        /// Method that write an error event in the website
        /// </summary>
        /// <param name="ex">Exception</param>
        public void writeException(Exception ex)
        {
            writeEvent(ex.Message + " - " + ex.StackTrace.ToString(), LogEvent.exc, entities);
        }

        protected ObjectId getId(string id)
        {
            return ObjectId.Parse(id);
        }
    }
}
