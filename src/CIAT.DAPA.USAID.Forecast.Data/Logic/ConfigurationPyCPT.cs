using CIAT.DAPA.USAID.Forecast.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    public partial class ConfigurationPyCPT
    {
        /// <summary>
        /// Method that return a string representation of the configuration cpt 
        /// </summary>
        /// <returns>String with the content of the configuration cpt</returns>
        public List<string> getTransformedTgts()
        {
            List<string> r = new List<string>();
            foreach (var t in tgts)
                r.Add(transformTgts(t));
            return r;
        }

        private string transformTgts(Quarter q)
        {
            string r = string.Empty;
            switch (q)
            {
                case Quarter.djf:
                    r = "Dec-Feb";
                    break;
                case Quarter.jfm:
                    r = "Jan-Mar";
                    break;
                case Quarter.fma:
                    r = "Feb-Apr";
                    break;
                case Quarter.mam:
                    r = "Mar-May";
                    break;
                case Quarter.amj:
                    r = "Apr-Jun";
                    break;
                case Quarter.mjj:
                    r = "May-Jul";
                    break;
                case Quarter.jja:
                    r = "Jun-Ago";
                    break;
                case Quarter.jas:
                    r = "Jul-Sep";
                    break;
                case Quarter.aso:
                    r = "Aug-Oct";
                    break;
                case Quarter.son:
                    r = "Sep-Nov";
                    break;
                case Quarter.ond:
                    r = "Oct-Dec";
                    break;
                case Quarter.ndj:
                    r = "Nov-Jan";
                    break;
            }
            return r;
        }
    }
}
