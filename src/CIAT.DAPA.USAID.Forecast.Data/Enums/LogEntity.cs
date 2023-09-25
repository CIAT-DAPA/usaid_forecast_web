using System;

namespace CIAT.DAPA.USAID.Forecast.Data.Enums
{
    /// <summary>
    /// This enumeration represents the entities that are affected in the system
    /// </summary>
    public enum LogEntity
    {
        /// <summary>
        /// Countries' table
        /// </summary>
        lc_country,
        /// <summary>
        /// States' table
        /// </summary>
        lc_state,
        /// <summary>
        /// Municipalities' table
        /// </summary>
        lc_municipality,
        /// <summary>
        /// Weather stations' table
        /// </summary>
        lc_weather_station,
        /// <summary>
        /// Setup table
        /// </summary>
        cp_setup,
        /// <summary>
        /// Crops' table
        /// </summary>
        cp_crop,
        /// <summary>
        /// Soils' table
        /// </summary>
        cp_soil,
        /// <summary>
        /// Cultivars' table
        /// </summary>
        cp_cultivar,
        /// <summary>
        /// Recommendation table
        /// </summary>
        cp_recommendation,
        /// <summary>
        /// Url table
        /// </summary>
        cp_url,
        /// <summary>
        /// Administrative log table
        /// </summary>
        log_administrative,
        /// <summary>
        /// Service log table
        /// </summary>
        log_service,
        /// <summary>
        /// Table of climatology
        /// </summary>
        hs_climatology,
        /// <summary>
        /// Climate History table
        /// </summary>
        hs_historical_climatic,
        /// <summary>
        /// Climate daily History table
        /// </summary>
        hs_historical_daily_data,
        /// <summary>
        /// Yield history table
        /// </summary>
        hs_historical_yield,
        /// <summary>
        /// Yield history table
        /// </summary>
        fs_forecast,
        /// <summary>
        /// Forecast scenario table
        /// </summary>
        fs_forecast_scenario,
        /// <summary>
        /// Yield forecast table
        /// </summary>
        fs_forecast_yield,
        /// <summary>
        /// Climate forecast table
        /// </summary>
        fs_forecast_climate,
        /// <summary>
        /// Phenological phase forecast table
        /// </summary>
        fs_forecast_phen_phase,
        /// <summary>
        /// Users table
        /// </summary>
        users,
        /// <summary>
        /// Roles table
        /// </summary>
        roles,
        /// <summary>
        /// Source table
        /// </summary>
        ad_source,
        /// <summary>
        /// User permission
        /// </summary>
        ad_user_permission
    }
}
