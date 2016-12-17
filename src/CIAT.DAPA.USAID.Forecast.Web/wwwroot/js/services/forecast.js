'use strict';

/**
 * @ngdoc service
 * @name ForecastApp.ForecastFactory
 * @description
 * # ForecastFactory
 * Factory in the ForecastApp.
 */
angular.module('ForecastApp')
    .factory('ForecastFactory', ['$http', 'config', function ($http, config) {
        var dataFactory = {};
        
        dataFactory.get = function () {
            if (dataFactory.raw == null) {
                dataFactory.raw = $http.get(config.api_forecast);
            }
            return dataFactory.raw;
        }
        return dataFactory;
    }]);
