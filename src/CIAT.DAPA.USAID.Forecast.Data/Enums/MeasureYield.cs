using System;

namespace CIAT.DAPA.USAID.Forecast.Data.Enums
{
    /// <summary>
    /// This enum represents the different variables available crop production on the platform
    /// </summary>
    public enum MeasureYield
    {
        /// <summary>
        /// yield kg/ha to 14% humidity
        /// </summary>
        yield_14,
        /// <summary>
        /// yield kg/ha to 0% humidity
        /// </summary>
        yield_0,
        /// <summary>
        /// days to harvest
        /// </summary>
        d_har,
        /// <summary>
        /// days to start grain drying
        /// </summary>
        d_dry,
        /// <summary>
        /// Cumulative precipitation for the crop cycle
        /// </summary>
        prec_acu,
        /// <summary>
        /// cumulative maximum temperature
        /// </summary>
        t_max_acu,
        /// <summary>
        /// cumulative minimum temperature
        /// </summary>
        t_min_acu,
        /// <summary>
        /// Total aboveground biomass accumulated
        /// </summary>
        bio_acu,
        /// <summary>
        /// Cumulative Evapotranspiration
        /// </summary>
        et_acu,
        /// <summary>
        /// Cumulative Evapotranspiration
        /// </summary>
        land_pre_day,
        /// <summary>
        /// nitrogen stress germination to booting
        /// </summary>
        st_ger_boo_n,
        /// <summary>
        /// nitrogen stress booting to anthesis
        /// </summary>
        st_boo_ant_n,
        /// <summary>
        /// nitrogen stress beginning to end of grain filling
        /// </summary>
        st_beg_end_gf_n,
        /// <summary>
        /// water stress germination to booting
        /// </summary>
        st_ger_boo_w,
        /// <summary>
        /// water stress booting to anthesis
        /// </summary>
        st_boo_ant_w,
        /// <summary>
        /// water stress beginning to end of grain filling
        /// </summary>
        st_beg_end_gf_w,
        /// <summary>
        /// nitrogen stress germination to anthesis
        /// </summary>
        st_ger_ant_n,
        /// <summary>
        /// water stress germination to anthesis
        /// </summary>
        st_ger_ant_w,
    }
}
