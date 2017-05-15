'use strict';

/**
 * @ngdoc service
 * @name ForecastApp.GeographicFactory
 * @description
 * # GeographicFactory
 * Geographic in the ForecastApp.
 */
angular.module('ForecastApp')
    .factory('ForecastApiFactory', function ($http, config) {
        var dataFactory = {
            cache: true,
            format: "json",
            raw_geographic: null,
            raw_geographic_crop: null,
            raw_agronomic:null,
            raw_forecast_climate: null,
            raw_forecast_yield: null,
            raw_history_climate: null,
            raw_history_climatology: null,
            raw_history_yield_year: null,
            raw_history_yield: null
        };

        /*
         * Method that initialize the forecast
         * (bool) cache: Set if the query is by cache
         * (string) format: Set the format to get the data
        */
        dataFactory.init = function (cache,format) {
            dataFactory.cache = cache;
            dataFactory.format = format;
        }

        /*
         * Method that return the url to get data geographic
        */
        dataFactory.getUrlGeographic = function () {
            return config.api_fs + "Geographic/" + dataFactory.format;
        }

        /*
        * Method that request all geographic information available from the forecast service
        */
        dataFactory.getGeographic = function () {
            if (!dataFactory.cache || (dataFactory.cache && dataFactory.raw_geographic == null)) {
                dataFactory.raw_geographic = $http.get(dataFactory.getUrlGeographic());
            }
            return dataFactory.raw_geographic;
        }

        /*
         * Method that return the url to get data agronomic
         * (bool) cultivar: True if request cultivar data, false if request soils data. It is only for the export data
        */
        dataFactory.getUrlAgronomic = function (cultivar) {
            return config.api_fs + "Agronomic/" + cultivar + "/" + dataFactory.format;
        }

        /*
        * Method that request all agronomic information available from the forecast service
        * (bool) cultivar: True if request cultivar data, false if request soils data. It is only for the export data
        */
        dataFactory.getAgronomic = function (cultivar) {
            if (!dataFactory.cache || (dataFactory.cache && dataFactory.raw_agronomic == null)) {
                dataFactory.raw_agronomic = $http.get(dataFactory.getUrlAgronomic(cultivar));
            }
            return dataFactory.raw_agronomic;
        }

        /*
        * Method that request all geographic information available from the forecast service
        * (string) crop: Crop name
        */
        dataFactory.getUrlGeographicCrop = function (crop) {
            return config.api_fs + "Geographic/Crop/" + crop + "/" + dataFactory.format;
        }

        /*
        * Method that request all geographic information available from the forecast service
        * (string) crop: Name of crop
        */
        dataFactory.getGeographicCrop = function (crop) {
            if (!dataFactory.cache || (dataFactory.cache && dataFactory.raw_geographic_crop == null)) {
                dataFactory.raw_geographic_crop = $http.get(dataFactory.getUrlGeographicCrop(crop));
            }
            return dataFactory.raw_geographic_crop;
        }

        /*
         * Method that return the url to get data geographic
         * (string) ws: Id weather station
         * (bool) probabilities: True for probabilities, false for scenarios
        */
        dataFactory.getUrlClimate = function (ws, probabilities) {
            return config.api_fs + "Forecast/Climate/" + ws + "/" + probabilities + "/" + dataFactory.format;
        }

        /*
        * Method that request all geographic information available from the forecast service
        * (string) ws: Id weather station
        * (bool) probabilities: True for probabilities, false for scenarios
        */
        dataFactory.getClimate = function (ws, probabilities) {
            if (!dataFactory.cache || (dataFactory.cache && dataFactory.raw_forecast_climate == null)) {
                dataFactory.raw_forecast_climate = $http.get(dataFactory.getUrlClimate(ws,probabilities));
            }
            return dataFactory.raw_forecast_climate;
        }

        /*
         * Method that return the url to get data climate historical
         * (string) ws: String with the id of the weather stations splited by comma
        */
        dataFactory.getUrlHistoricalClimate = function (ws) {
            return config.api_fs + "Historical/HistoricalClimatic/" + ws + "/" + dataFactory.format;
        }

        /*
        * Method that request the climate historical data from the web api
        * (string) ws: String with the id of the weather stations splited by comma
        */
        dataFactory.getHistoricalClimate = function (ws) {
            if (!dataFactory.cache || (dataFactory.cache && dataFactory.raw_history_climate == null))
                dataFactory.raw_history_climate = $http.get(dataFactory.getUrlHistoricalClimate(ws));
            return dataFactory.raw_history_climate;
        }

        /*
         * Method that return the url to get data climate historical
         * (string) ws: String with the id of the weather stations splited by comma
        */
        dataFactory.getUrlHistoricalClimatology = function (ws) {
            return config.api_fs + "Historical/Climatology/" + ws + "/" + dataFactory.format;
        }

        /*
        * Method that request the climate historical data from the web api
        * (string) ws: String with the id of the weather stations splited by comma
        */
        dataFactory.getHistoricalClimatology = function (ws) {
            if (!dataFactory.cache || (dataFactory.cache && dataFactory.raw_history_climatology == null))
                dataFactory.raw_history_climatology = $http.get(dataFactory.getUrlHistoricalClimatology(ws));
            return dataFactory.raw_history_climatology;
        }

        /*
         * Method that return the url to get data forecast
         * (string) ws: Id weather station
        */
        dataFactory.getUrlForecastYield = function (ws) {
            return config.api_fs + "Forecast/Yield/" + ws + "/" + dataFactory.format;
        }

        /*
        * Method that request the last forecast to the web api
        * (string) ws: Id weather station
        */
        dataFactory.getForecastYield = function (ws) {
            if (!dataFactory.cache || (dataFactory.cache && dataFactory.raw_forecast_yield == null))
                dataFactory.raw_forecast_yield = $http.get(dataFactory.getUrlForecastYield(ws));
            return dataFactory.raw_forecast_yield;
        }

        /*
         * Method that return the url to get yield historical years
         * (string) ws: Concatenate string with ids of the weather stations         
        */
        dataFactory.getUrlHistoricalYieldYears = function (ws) {
            return config.api_fs + "Historical/HistoricalYieldYears/" + ws + "/" + dataFactory.format;
        }

        /*
        * Method that request the years available with information for a weather station
        * (string) ws: String with the id of the weather stations splited by comma
        */
        dataFactory.getHistoricalYieldYears = function (ws) {
            if (!dataFactory.cache || (dataFactory.cache && dataFactory.raw_history_yield_year == null))
                dataFactory.raw_history_yield_year = $http.get(dataFactory.getUrlHistoricalYieldYears(ws));
            return dataFactory.raw_history_yield_year;
        }

        /*
         * Method that return the url to get yield historical
         * (string) ws: Concatenate string with ids of the weather stations
         * (string) years: Concatenate string with years to search data
        */
        dataFactory.getUrlHistoricalYield = function (ws, years) {
            return config.api_fs + "Historical/HistoricalYield/" + ws + "/" + years + "/" + dataFactory.format;
        }

        /*
        * Method that request the years available with information for a weather station
        * (string) ws: String with the id of the weather stations splited by comma
        * (string) years: Concatenate string with years to search data
        */
        dataFactory.getHistoricalYield = function (ws, years) {
            if (!dataFactory.cache || (dataFactory.cache && dataFactory.raw_history_yield == null))
                dataFactory.raw_history_yield = $http.get(dataFactory.getUrlHistoricalYield(ws, years));
            return dataFactory.raw_history_yield;
        }

        return dataFactory;
    });