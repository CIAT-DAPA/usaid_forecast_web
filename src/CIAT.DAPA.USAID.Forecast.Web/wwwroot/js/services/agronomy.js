'use strict';

/**
 * @ngdoc service
 * @name ForecastApp.ForecastFactory
 * @description
 * # ForecastFactory
 * Factory in the ForecastApp.
 */
angular.module('ForecastApp')
    .factory('AgronomyFactory', function ($http, config) {
        var dataFactory = { cache: true, raw: null};

        /*
         * Method that return the url to get data geographic
        */
        dataFactory.getUrl = function () {
            return config.api_fs + config.api_fs_agronomic;
        }

        /*
        * Method that request all agronomic information available from the forecast service
        */
        dataFactory.get = function () {
            if (dataFactory.raw == null) {
                dataFactory.raw = $http.get(dataFactory.getUrl());
            }
            return dataFactory.raw;
        }
        return dataFactory;
    });