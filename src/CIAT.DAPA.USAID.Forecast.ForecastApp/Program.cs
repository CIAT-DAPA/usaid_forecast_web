﻿using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.ForecastApp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using CIAT.DAPA.USAID.Forecast.ForecastApp.Models.Tools;
using System.IO;
using CIAT.DAPA.USAID.Forecast.Data.Database;

namespace CIAT.DAPA.USAID.Forecast.ForecastApp
{
    public class Program
    {
        public static Settings settings { get; set; }

        public static void Main(string[] args)
        {
            try
            {
                // Load the configuration file
                /*var builder = new ConfigurationBuilder()
                    .AddJsonFile($"appsettings.json", true, true)
                    .AddEnvironmentVariables();*/
                string dir = Directory.GetCurrentDirectory();
                //dir = dir.Contains("forecast_app") ? dir : dir + "\\forecast_app";
                var builder = new ConfigurationBuilder()
                                    .SetBasePath(dir)                                    
                                    .AddJsonFile("appsettings.json", optional: true,reloadOnChange: true);
                var conf = builder.Build();
                Console.WriteLine("Working in: " + dir);
                Program.settings = new Settings()
                {
                    splitted = ',',
                    ConnectionString = conf["ConnectionString"],
                    Database = conf["Database"],
                    Out_PATH_FS_FILES = conf["Out_PATH_FS_FILES"],
                    Out_PATH_STATES = conf["Out_PATH_STATES"],
                    Out_PATH_WS_FILES = conf["Out_PATH_WS_FILES"],
                    Out_CROPS_COORDINATES = conf["Out_CROPS_COORDINATES"].Split(','),
                    Out_PATH_FILE_COORDINATES = conf["Out_PATH_FILE_COORDINATES"],
                    Out_PATH_USERS = conf["Out_PATH_USERS"],
                    In_PATH_FS_PROBABILITIES = conf["In_PATH_FS_PROBABILITIES"],
                    In_PATH_FS_FILE_PROBABILITY = conf["In_PATH_FS_FILE_PROBABILITY"],
                    In_PATH_FS_RASTER_SOURCE = conf["In_PATH_FS_RASTER_SOURCE"],
                    In_PATH_FS_RASTER_DESTINATION = conf["In_PATH_FS_RASTER_DESTINATION"],
                    In_PATH_FS_FILE_PERFORMANCE = conf["In_PATH_FS_FILE_PERFORMANCE"],
                    In_PATH_FS_SCENARIOS = conf["In_PATH_FS_SCENARIOS"],
                    In_PATH_FS_D_SCENARIO = conf["In_PATH_FS_D_SCENARIO"],
                    In_PATH_FS_YIELD = conf["In_PATH_FS_YIELD"],
                    In_PATH_FS_CLIMATE = conf["In_PATH_FS_CLIMATE"],
                    Social_Network_Message = conf["Social_Network_Message"],
                    Social_Network_Twitter_AccessToken = conf["Social_Network_Twitter_AccessToken"],
                    Social_Network_Twitter_AccessTokenSecret = conf["Social_Network_Twitter_AccessTokenSecret"],
                    Social_Network_Twitter_ConsumerKey = conf["Social_Network_Twitter_ConsumerKey"],
                    Social_Network_Twitter_ConsumerKeySecret = conf["Social_Network_Twitter_ConsumerKeySecret"],
                    Add_Day = bool.Parse(conf["Add_Day"])
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading appsettings.json");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            // Execute the program
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            try
            {
                if (Program.searchParameter(args, "-test") == 0)
                {
                    await (new ForecastDB(Program.settings.ConnectionString, Program.settings.Database)).testConnectionAsync();
                }
                else if (Program.searchParameter(args, "-share") == 0)
                {
                    CSocialNetworks socialNetworks = new CSocialNetworks();
                    await socialNetworks.shareForecast();
                }
                // Check the first parameter to validate if the action is export (-out) or import (-in)
                else if (Program.searchParameter(args, "-out") == 0)
                {
                    int path = Program.searchParameter(args, "-p");
                    Program.validateParameter(path, "-p");
                    COut output = new COut();
                    // Export states
                    // -out -s "prec" -p "C:\Users\hsotelo\Desktop\test export\\" -start 1981 -end 2013
                    int s = Program.searchParameter(args, "-s");
                    if (s >= 0)
                    {
                        int start = Program.searchParameter(args, "-start");
                        Program.validateParameter(start, "-start");
                        int end = Program.searchParameter(args, "-end");
                        Program.validateParameter(end, "-end");

                        Console.WriteLine("Exporting historical climate by states");
                        await output.exportStatesHistoricalClimateAsync(args[path + 1], (MeasureClimatic)Enum.Parse(typeof(MeasureClimatic), args[s + 1], true), int.Parse(args[start + 1]), int.Parse(args[end + 1]));
                    }
                    // Export configuration files from weather stations
                    // -out -wf -p "C:\Users\hsotelo\Desktop\test export\\" -name "daily"
                    int wf = Program.searchParameter(args, "-wf");
                    if (wf >= 0)
                    {
                        int name = Program.searchParameter(args, "-name");
                        Program.validateParameter(name, "-name");
                        Console.WriteLine("Exporting configuration file by weather station");
                        await output.exportFilesWeatherStationAsync(args[path + 1], args[name + 1]);
                    }
                    // Export coords from weather stations
                    // -out -co -p "C:\Users\hsotelo\Desktop\test export\\" -name "daily"
                    int co = Program.searchParameter(args, "-co");
                    if (co >= 0)
                    {
                        Console.WriteLine("Exporting coordinates of the weather stations");
                        await output.exportCoordsWeatherStationAsync(args[path + 1]);
                    }
                    // Export forecast setup
                    // -out -fs -p "C:\Users\hsotelo\Desktop\test export\\"
                    int fs = Program.searchParameter(args, "-fs");
                    if (fs >= 0)
                    {
                        Console.WriteLine("Exporting forecast setup");
                        await output.exportForecastSetupAsync(args[path + 1]);
                    }
                    // Export cpt setup
                    // -out -cpt -p "C:\Users\hsotelo\Desktop\test export\\"
                    int cpt = Program.searchParameter(args, "-cpt");
                    if (cpt >= 0)
                    {
                        Console.WriteLine("Exporting CPT setup");
                        await output.exportCPTSetupAsync(args[path + 1]);
                    }
                    // Export emails users
                    // -out -usr -p "C:\Users\hsotelo\Desktop\test export\\"
                    int usr = Program.searchParameter(args, "-usr");
                    if (usr >= 0)
                    {
                        Console.WriteLine("Exporting Users");
                        await output.exportUsersEmailsAsync(args[path + 1]);
                    }
                }
                else if (Program.searchParameter(args, "-in") == 0)
                {
                    int path = Program.searchParameter(args, "-p");
                    Program.validateParameter(path, "-p");
                    CIn cin = new CIn();
                    // Import forecast
                    // -in -fs -p "C:\Users\hsotelo\Desktop\test export\\" -cf 0.5
                    int fs = Program.searchParameter(args, "-fs");
                    if (fs >= 0)
                    {
                        int cf = Program.searchParameter(args, "-cf");
                        Program.validateParameter(cf, "-cf");
                        Console.WriteLine("Importing forecast");
                        await cin.importForecastAsync(args[path + 1], double.Parse(args[cf + 1]));
                    }
                    //-in -hs -s "prec" -type 1 -p "C:\data.csv" 
                    int hs = Program.searchParameter(args, "-hs");
                    if (hs >= 0)
                    {
                        int s = Program.searchParameter(args, "-s");
                        Program.validateParameter(s, "-s");
                        int type = Program.searchParameter(args, "-type");
                        Program.validateParameter(type, "-type");
                        Console.WriteLine("Importing historical data");
                        await cin.importHistoricalAsync(args[path + 1], (MeasureClimatic)Enum.Parse(typeof(MeasureClimatic), args[s + 1]), int.Parse(args[type + 1]));
                    }
                }
                else if (Program.searchParameter(args, "-help") == 0)
                {
                    foreach (var l in File.ReadAllLines("help.txt"))
                        Console.WriteLine(l);
                }
                else
                {
                    throw new Exception("The first parameter should indicate if you want export (-out) or import (-in).\nRecommend execute with parameter -help to get more information");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("For more information you can execute the following command:");
                Console.WriteLine("-help");
            }
        }

        /// <summary>
        /// Method that search a value into parameters
        /// </summary>
        /// <param name="args">Arguments list</param>
        /// <param name="name">Name of parameter</param>
        /// <returns>If the answer is -1, the paramter doesn't exist, otherwise, represents the index of paramater</returns>
        public static int searchParameter(string[] args, string name)
        {
            for (int i = 0; i < args.Length; i++)
                if (args[i].Trim().ToLower().Equals(name.Trim().ToLower()))
                    return i;
            return -1;
        }

        /// <summary>
        /// Method that validates the input parameter
        /// </summary>
        /// <param name="index">Position in the args</param>
        /// <param name="name">Name of parameter</param>
        public static void validateParameter(int index, string name)
        {
            if (index < 0)
                throw new Exception("The parameter " + name + " is mandatory");
        }
    }
}
