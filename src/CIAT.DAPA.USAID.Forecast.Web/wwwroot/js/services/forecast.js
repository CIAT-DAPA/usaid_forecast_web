'use strict';

/**
 * @ngdoc service
 * @name ForecastApp.ForecastFactory
 * @description
 * # ForecastFactory
 * Factory in the ForecastApp.
 */
angular.module('ForecastApp')
    .factory('ForecastFactory', function ($http, config) {
        var dataFactory = {
            cache: true,
            raw_climate: null,
            raw_yield: null
        };

        /*
         * Method that return the url to get data forecast
        */
        dataFactory.getUrlClimate = function () {
            return config.api_fs + config.api_fs_forecast_climate;
        }

        /*
        * Method that request the last forecast to the web api
        */
        dataFactory.getClimate = function () {
            if (!dataFactory.cache || (dataFactory.cache && dataFactory.raw_climate == null))
                dataFactory.raw_climate = $http.get(dataFactory.getUrlClimate());
            return dataFactory.raw_climate;
        }

        /*
         * Method that return the url to get data forecast
        */
        dataFactory.getUrlYield = function () {
            return config.api_fs + config.api_fs_forecast_yield;
        }

        /*
        * Method that request the last forecast to the web api
        */
        dataFactory.getYield = function () {
            if (!dataFactory.cache || (dataFactory.cache && dataFactory.raw_yield == null))
                dataFactory.raw_yield = $http.get(dataFactory.getUrlYield());
            return dataFactory.raw_yield;
        }

        return dataFactory;
    });
