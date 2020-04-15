using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.WebAdmin.Models.Tools
{
    /// <summary>
    /// This class load the settings of the web site from the appsettings.json
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// String to connect with the database
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// Name of the database
        /// </summary>
        public string Database { get; set; }
        /// <summary>
        /// Path where the log is located
        /// </summary>
        public string LogPath { get; set; }
        /// <summary>
        /// Path where the import files are located
        /// </summary>
        public string ImportPath { get; set; }
        /// <summary>
        /// Path where the configuration files are located
        /// </summary>
        public string ConfigurationPath { get; set; }
        /// <summary>
        /// Get or set if the application was installed
        /// </summary>
        public bool Installed { get; set; }
        /// <summary>
        /// Get or set the email account of the notification
        /// </summary>
        public string NotifyAccount { get; set; }
        /// <summary>
        /// Get or set the password of the email account for the notifications
        /// </summary>
        public string NotifyPassword { get; set; }
        /// <summary>
        /// Get or set the server for the notifications
        /// </summary>
        public string NotifyServer { get; set; }
        /// <summary>
        /// Get or set the port of the notification server
        /// </summary>
        public int NotifyPort { get; set; }
        /// <summary>
        /// Get or set if the notification server uses ssl
        /// </summary>
        public bool NotifySsl { get; set; }
        /// <summary>
        /// Get or set the languages available
        /// </summary>
        public string[] Languages { get; set; }
        /// <summary>
        /// Get or set the crops available
        /// </summary>
        public string[] Crops { get; set; }
    }
}
