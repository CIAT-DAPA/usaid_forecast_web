using CIAT.DAPA.USAID.Forecast.Data.Enums;
using CIAT.DAPA.USAID.Forecast.ForecastApp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.ForecastApp
{
    public class Program
    {
        public static string cnn { get; set; }
        public static string db { get; set; }
        public static void Main(string[] args)
        {
            Program.cnn = "mongodb://localhost:27017";
            Program.db = "forecast_db";
            MainAsync(args).GetAwaiter().GetResult();
            Console.WriteLine();
            Console.WriteLine("Press Enter");
            Console.ReadLine();
        }

        static async Task MainAsync(string[] args)
        {
            try
            {
                int path;
                // Check the first parameter to validate if the action is export (-out) or import (-in)
                if (Program.searchParameter(args, "-out") == 0)
                {
                    COut output = new COut();
                    // Get 
                    path = Program.searchParameter(args, "-p");
                    Program.validateParameter(path, "-p");
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
                    // Export forecast setup
                    // -out -fs -p "C:\Users\hsotelo\Desktop\test export\\"
                    int fs = Program.searchParameter(args, "-fs");
                    if (fs >= 0)
                    {                        
                        Console.WriteLine("Exporting forecast setup");
                        //await output.exportFilesWeatherStationAsync(args[path + 1], args[name + 1]);
                    }
                }
                else if (Program.searchParameter(args, "-in") == 0)
                {

                }
                else if (Program.searchParameter(args, "-help") == 0)
                {
                    Console.WriteLine("Help Menu");
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
