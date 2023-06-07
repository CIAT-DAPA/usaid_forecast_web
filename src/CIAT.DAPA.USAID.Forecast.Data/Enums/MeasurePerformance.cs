using System;

namespace CIAT.DAPA.USAID.Forecast.Data.Enums
{
    /// <summary>
    /// This enum represents the different metrics of climate models
    /// </summary>
    public enum MeasurePerformance
    {
        goodness,
        kendall,
        pearson,
        canonica,
        afc2,
        groc,
        ignorance,
        rpss,
        spearman
    }
}
