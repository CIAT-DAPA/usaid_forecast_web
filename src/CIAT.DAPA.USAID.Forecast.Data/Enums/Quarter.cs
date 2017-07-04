using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIAT.DAPA.USAID.Forecast.Data.Enums
{
    /// <summary>
    /// This enumeration represents the quarters of the climatic forecast
    /// </summary>
    public enum Quarter
    {
        /// <summary>
        /// December January February
        /// </summary>
        djf,
        /// <summary>
        /// January February March
        /// </summary>
        jfm,
        /// <summary>
        /// February March April
        /// </summary>
        fma,
        /// <summary>
        /// March April May
        /// </summary>
        mam,
        /// <summary>
        /// April May June
        /// </summary>
        amj,
        /// <summary>
        /// May June July
        /// </summary>
        mjj,
        /// <summary>
        /// June July August
        /// </summary>
        jja,
        /// <summary>
        /// July August September
        /// </summary>
        jas,
        /// <summary>
        /// August September October
        /// </summary>
        aso,
        /// <summary>
        /// September October November
        /// </summary>
        son,
        /// <summary>
        /// October November December
        /// </summary>
        ond,
        /// <summary>
        /// November December January
        /// </summary>
        ndj
    }
}
