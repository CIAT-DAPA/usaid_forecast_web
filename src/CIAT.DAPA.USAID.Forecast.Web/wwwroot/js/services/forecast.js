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

        
        return dataFactory;
    });
