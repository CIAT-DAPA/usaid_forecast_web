'use strict';

/**
 * @ngdoc service
 * @name ForecastApp.GeographicFactory
 * @description
 * # GeographicFactory
 * Geographic in the ForecastApp.
 */
angular.module('ForecastApp')
    .factory('GeographicFactory', function ($http, config) {
        var dataFactory = { raw: null, cache: true };

        /*
         * Method that return the url to get data geographic
        */
        dataFactory.getUrl = function () {
            return config.api_fs + config.api_fs_geographic;
        }
        /*
        * Method that request all geographic information available from the forecast service
        */
        dataFactory.get = function () {
            if (!dataFactory.cache || (dataFactory.cache && dataFactory.raw == null)) {
                dataFactory.raw = $http.get(dataFactory.getUrl());
            }
                
            return dataFactory.raw;
        }
        return dataFactory;
    });