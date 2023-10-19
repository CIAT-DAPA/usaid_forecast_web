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
        /// Land preparation day
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
        /// <summary>
        /// Hydrological balance between sowing and emergence
        /// </summary>
        hs_hb_s_e,
        /// <summary>
        /// Hydrological balance during tillering
        /// </summary>
        hs_hb_t,
        /// <summary>
        /// Hydrological balance between  beginning of stem elongation period and end of booting
        /// </summary>
        hs_hb_ei_b,
        /// <summary>
        /// Hydrological balance between beginning of heading and full maturity
        /// </summary>
        hs_hb_bh_m,
        /// <summary>
        /// Hydrological balance between sowing and full maturity
        /// </summary>
        hs_hb_s_m,
        /// <summary>
        /// Rainfall amount during pre-sowing period
        /// </summary>
        hs_ra_s,
        /// <summary>
        /// Number of rainy days with rain above 10 mm  during tillering
        /// </summary>
        hs_ndr10_t,
        /// <summary>
        /// Number of rainy days with rain above 40 mm  during tillering
        /// </summary>
        hs_ndr40_t,
        /// <summary>
        /// Number of days with rain above 5 mm between heading and full maturity
        /// </summary>
        hs_ndr5_h_m,
        /// <summary>
        /// Number of days with rain above 40 mm between heading and full maturity
        /// </summary>
        hs_ndr40_bh_m,
        /// <summary>
        /// Maximum number of consecutive days with rain above 5 mm between heading and flowering
        /// </summary>
        hs_cdr5_h_f,
        /// <summary>
        /// Maximum number of consecutive days with rain above 5 mm between flowering and full maturity
        /// </summary>
        hs_cdr5_f_m,
        /// <summary>
        /// Number of days with minimum daily temperature below 2 °C between booting and flowering
        /// </summary>
        hs_ndt2_b_f,
        /// <summary>
        /// Number of hot days with maximum daily temperature above 28 °C between beginning of stem elongation period and flowering
        /// </summary>
        hs_ndt28_b_f,
        /// <summary>
        /// 
        /// </summary>
        hs_hb_0,
        /// <summary>
        /// 
        /// </summary>
        hs_hb_1,
        /// <summary>
        /// 
        /// </summary>
        hs_hb_2,
        /// <summary>
        /// 
        /// </summary>
        hs_hb_3,
        /// <summary>
        /// 
        /// </summary>
        hs_hb_4,
        /// <summary>
        /// 
        /// </summary>
        hs_hb_5,
        /// <summary>
        /// 
        /// </summary>
        hs_ndr40_0,
        /// <summary>
        /// 
        /// </summary>
        hs_ndr40_1,
        /// <summary>
        /// 
        /// </summary>
        hs_ndr40_2,
        /// <summary>
        /// 
        /// </summary>
        hs_ndr40_3,
        /// <summary>
        /// 
        /// </summary>
        hs_ndr40_4,
        /// <summary>
        /// 
        /// </summary>
        hs_ndr40_5,
        /// <summary>
        /// 
        /// </summary>
        hs_ndr5_0,
        /// <summary>
        /// 
        /// </summary>
        hs_ndr5_1,
        /// <summary>
        /// 
        /// </summary>
        hs_ndr5_2,
        /// <summary>
        /// 
        /// </summary>
        hs_ndr5_3,
        /// <summary>
        /// 
        /// </summary>
        hs_ndr5_4,
        /// <summary>
        /// 
        /// </summary>
        hs_ndr5_5,
        /// <summary>
        /// 
        /// </summary>
        hs_cdr5_0,
        /// <summary>
        /// 
        /// </summary>
        hs_cdr5_1,
        /// <summary>
        /// 
        /// </summary>
        hs_cdr5_2,
        /// <summary>
        /// 
        /// </summary>
        hs_cdr5_3,
        /// <summary>
        /// 
        /// </summary>
        hs_cdr5_4,
        /// <summary>
        /// 
        /// </summary>
        hs_cdr5_5,
        /// <summary>
        /// 
        /// </summary>
        hs_ndt33_0,
        /// <summary>
        /// 
        /// </summary>
        hs_ndt33_1,
        /// <summary>
        /// 
        /// </summary>
        hs_ndt33_2,
        /// <summary>
        /// 
        /// </summary>
        hs_ndt33_3,
        /// <summary>
        /// 
        /// </summary>
        hs_ndt33_4,
        /// <summary>
        /// 
        /// </summary>
        hs_ndt33_5,
        /// <summary>
        /// rainy_days
        /// </summary>
        ag_ndll,
        /// <summary>
        /// dry_days
        /// </summary>
        ag_nds,
        /// <summary>
        /// heat_days
        /// </summary>
        ag_ndc,
        /// <summary>
        /// cold_days
        /// </summary>
        ag_ndf,
        /// <summary>
        /// drought_days
        /// </summary>
        ag_ndd,
        /// <summary>
        /// heat_drought_days
        /// </summary>
        ag_dcdh,
        /// <summary>
        /// total_rainfall
        /// </summary>
        ag_llta,
    }
}
