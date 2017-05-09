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
        var dataFactory = { raw: null, cache: true };

        /*
         * Method that return the url to get data forecast
        */
        dataFactory.getUrl = function () {
            return config.api_fs + config.api_fs_forecast;
        }
        /*
        * Method that request the last forecast to the web api
        */
        dataFactory.get = function () {
            if (!dataFactory.cache || (dataFactory.cache && dataFactory.raw == null))
                dataFactory.raw = $http.get(dataFactory.getUrl());
            return dataFactory.raw;
        }
        return dataFactory;
    });
