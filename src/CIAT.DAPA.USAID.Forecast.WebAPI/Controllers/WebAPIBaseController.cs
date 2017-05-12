using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CIAT.DAPA.USAID.Forecast.Data.Database;
using CIAT.DAPA.USAID.Forecast.WebAPI.Models.Tools;
using CIAT.DAPA.USAID.Forecast.Data.Enums;
using Microsoft.Extensions.Options;
using MongoDB.Bson;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CIAT.DAPA.USAID.Forecast.WebAPI.Controllers
{
    
    public abstract class WebAPIBaseController : Controller
    {
        /// <summary>
        /// Get or set object to connect with database
        /// </summary>
        protected ForecastDB db { get; set; }
        /// <summary>
        /// Get or set object to write the events occurring on the web api 
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
        /// String to delimiter the file csv
        /// </summary>
        protected string delimiter { get; set; }

        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Settings options</param>
        public WebAPIBaseController(IOptions<Settings> settings, LogEntity entity) : base()
        {
            init(settings, new List<LogEntity>() { entity });
        }

        /// <summary>
        /// Method Construct
        /// </summary>
        /// <param name="settings">Settings options</param>
        public WebAPIBaseController(IOptions<Settings> settings, List<LogEntity> list_entities) : base()
        {
            init(settings, list_entities);
        }

        /// <summary>
        /// Method to init the web api controller
        /// </summary>
        /// <param name="settings">Settings options</param>
        /// <param name="list_entities">List of entities affected</param>
        private void init(IOptions<Settings> settings, List<LogEntity> list_entities)
        {
            db = new ForecastDB(settings.Value.ConnectionString, settings.Value.Database);
            log = new Log(settings, db.logService);
            entities = list_entities;            
            delimiter = settings.Value.Delimiter;
        }

        /// <summary>
        /// Method that write event in the log
        /// </summary>
        /// <param name="content">Description event</param>
        /// <param name="e">Type event</param>
        /// <param name="entities_affected">List of the entities affected</param>
        public void writeEvent(string content, LogEvent e, List<LogEntity> entities_affected)
        {
            user = Request.HttpContext.Connection.RemoteIpAddress.ToString();
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

        /// <summary>
        /// This method translate from string to object id
        /// </summary>
        /// <param name="id">string data</param>
        /// <returns>Object Id</returns>
        protected ObjectId getId(string id)
        {
            return ObjectId.Parse(id);
        }
    }
}
