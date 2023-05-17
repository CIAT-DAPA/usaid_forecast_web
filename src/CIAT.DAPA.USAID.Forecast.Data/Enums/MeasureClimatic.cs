using System;

namespace CIAT.DAPA.USAID.Forecast.Data.Enums
{
    /// <summary>
    /// This enum represents the different climatic variables available on the platform
    /// </summary>
    public enum MeasureClimatic
    {
        /// <summary>
        /// Precipitation
        /// </summary>
        prec,
        /// <summary>
        /// Maximum temperature
        /// </summary>
        t_max,
        /// <summary>
        /// minimum temperature
        /// </summary>
        t_min,
        /// <summary>
        /// Relative humidity
        /// </summary>
        rel_hum,
        /// <summary>
        /// Solar radiation
        /// </summary>
        sol_rad,
        /// <summary>
        /// Lower tertile of normality of precipitation
        /// </summary>
        prec_ter_1,
        /// <summary>
        /// Upper tertile of normality of precipitation
        /// </summary>
        prec_ter_2,



    }
}
