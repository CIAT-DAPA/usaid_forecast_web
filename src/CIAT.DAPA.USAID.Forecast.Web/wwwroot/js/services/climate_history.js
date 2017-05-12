'use strict';

/**
 * @ngdoc service
 * @name ForecastApp.ClimateHistoryFactory
 * @description
 * # ClimateHistoryFactory
 * Climate History in the ForecastApp.
 */
angular.module('ForecastApp')
    .factory('ClimateHistoryFactory', function ($http, config) {
        var dataFactory = { raw: null, cache: true };

        /*
         * Method that return the url to get data climate historical
         * (string) ws: String with the id of the weather stations splited by comma
        */
        dataFactory.getUrl = function (ws) {
            return config.api_fs + config.api_fs_historical + ws;
        }

        /*
        * Method that request the climate historical data from the web api
        * (string) ws: String with the id of the weather stations splited by comma
        */
        dataFactory.get = function (ws) {
            if (!dataFactory.cache || (dataFactory.cache && dataFactory.raw == null))
                dataFactory.raw = $http.get(dataFactory.getUrl(ws));
            return dataFactory.raw;
        }
        return dataFactory;
    });
