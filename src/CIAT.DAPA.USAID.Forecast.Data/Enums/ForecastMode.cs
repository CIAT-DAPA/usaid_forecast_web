using System;

namespace CIAT.DAPA.USAID.Forecast.Data.Enums
{
    /// <summary>
    /// This enumeration represents the mode to execute climate forecast
    /// </summary>
    public enum ForecastMode
    {
        /// <summary>
        /// No execute forecast
        /// </summary>
        none,
        /// <summary>
        /// Use CPT traditional
        /// </summary>
        cpt,
        /// <summary>
        /// Use next gen scripts
        /// </summary>
        next_gen
    }
}
