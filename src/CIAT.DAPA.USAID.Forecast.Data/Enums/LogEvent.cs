using System;

namespace CIAT.DAPA.USAID.Forecast.Data.Enums
{
    /// <summary>
    /// This enum represents the events that can be performed on the application
    /// </summary>
    public enum LogEvent
    {
        /// <summary>
        /// Evento to create a record
        /// </summary>
        cre,
        /// <summary>
        /// Event to search records
        /// </summary>
        rea,
        /// <summary>
        /// Event to update records
        /// </summary>
        upd,
        /// <summary>
        /// Event to delete records
        /// </summary>
        del,
        /// <summary>
        /// Event to list records
        /// </summary>
        lis,
        /// <summary>
        /// Error in the application
        /// </summary>
        err,
        /// <summary>
        /// Exception in the application
        /// </summary>
        exc
    }
}
