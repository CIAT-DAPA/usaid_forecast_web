using CIAT.DAPA.USAID.Forecast.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CIAT.DAPA.USAID.Forecast.Data.Models
{
    public partial class ConfigurationPyCPT
    {        
        /// <summary>
        /// Method that transform a ModelsPyCPT into a correct string output
        /// </summary>
        /// <param name="m">Model that you want to get the correct name</param>
        /// <returns>string</returns>
        public static string getNameModelsPyCpt(ModelsPyCpt m)
        {
            string val = string.Empty;
            if (m == ModelsPyCpt.CanSIPSv2)
                val = "CanSIPSv2";
            else if (m == ModelsPyCpt.COLA_RSMAS_CCSM4)
                val = "COLA-RSMAS-CCSM4";
            else if (m == ModelsPyCpt.GFDL_CM2p5_FLOR_A06)
                val = "GFDL-CM2p5-FLOR-A06";
            else if (m == ModelsPyCpt.GFDL_CM2p5_FLOR_B01)
                val = "GFDL-CM2p5-FLOR-B01";
            else if (m == ModelsPyCpt.NASA_GEOSS2S)
                val = "NASA-GEOSS2S";
            else if (m == ModelsPyCpt.NCEP_CFSv2)
                val = "NCEP-CFSv2";
            else if (m == ModelsPyCpt.EU_C3S_ECMWF_SEAS5)
                val = "EU-C3S-ECMWF-SEAS5";
            else if (m == ModelsPyCpt.EU_C3S_MeteoFrance_System7)
                val = "EU-C3S-MeteoFrance-System7";
            else if (m == ModelsPyCpt.EU_C3S_UKMO_GloSea6GC2S600)
                val = "EU-C3S-UKMO-GloSea6GC2S600";
            else if (m == ModelsPyCpt.EU_C3S_DWD_GCFS2p1)
                val = "EU-C3S-DWD-GCFS2p1";
            else if (m == ModelsPyCpt.EU_C3S_CMCC_SPS3p5)
                val = "EU-C3S-CMCC-SPS3p5";
            else if (m == ModelsPyCpt.ECMWF)
                val = "ECMWF";
            else if (m == ModelsPyCpt.CFSv2_SubX)
                val = "CFSv2_SubX";
            return val;
        }

        /// <summary>
        /// Method that return the correct name for obs
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static string getNameObs(Obs m)
        {
            string val = string.Empty;
            if (m == Obs.CPC_CMAP_URD)
                val = "CPC_CMAP_URD";
            else if (m == Obs.CHIRPS)
                val = "CHIRPS";
            else if (m == Obs.TRMM)
                val = "TRMM";
            else if (m == Obs.CPC)
                val = "CPC";
            else if (m == Obs.Chilestations)
                val = "Chilestations";
            else if (m == Obs.ENACT)
                val = "ENACT";
            return val;
        }

        /// <summary>
        /// Method that return the correct name for Mos
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static string getNameMos(Mos m)
        {
            string val = string.Empty;
            if (m == Mos.PCR)
                val = "PCR";
            else if (m == Mos.CCA)
                val = "CCA";
            else if (m == Mos.None)
                val = "None";
            return val;
        }

        /// <summary>
        /// Method that return the correct name for Predictors
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static string getNamePredictors(Predictors m)
        {
            string val = string.Empty;
            if (m == Predictors.PRCP)
                val = "PRCP";
            else if (m == Predictors.GCM)
                val = "GCM";
            else if (m == Predictors.VQ)
                val = "VQ";
            else if (m == Predictors.UQ)
                val = "UQ";
            else if (m == Predictors.T2M)
                val = "T2M";
            return val;
        }

        /// <summary>
        /// Method that return the correct name for Predictand
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static string getNamePredictand(Predictand m)
        {
            string val = string.Empty;
            if (m == Predictand.PRCP)
                val = "PRCP";
            else if (m == Predictand.RFREQ)
                val = "RFREQ";
            return val;
        }

        /// <summary>
        /// Method that return all models pycpt in string list
        /// </summary>
        /// <returns>List<string></string></returns>
        public List<string> getModelsPyCPT()
        {
            List<string> val = new List<string>();
            foreach (var m in this.models)
                val.Add(ConfigurationPyCPT.getNameModelsPyCpt(m));
            return val;
        }
    }
}
