'use strict';

/**
 * @ngdoc service
 * @name ForecastApp.ForecastFactory
 * @description
 * # ForecastFactory
 * Factory in the ForecastApp.
 */
angular.module('ForecastApp')
    .factory('GeographicFactory', ['$http', 'config', function ($http, config) {
        var dataFactory = {};
        /*
        * Method that request all geographic information available from the forecast service
        */
        dataFactory.get = function () {
            if (dataFactory.raw == null) {
                dataFactory.raw = $http.get(config.api_fs + config.api_fs_geographic);
            }
            return dataFactory.raw;
        }
        return dataFactory;
    }])
    .factory('MunicipalityFactory', ['config', function (config) {
        var dataFactory = {};
        
        /*
        * Method that filter all climate data from forecast of the weather station
        * (object) raw: Json with all geographic data
        */
        dataFactory.getByWeatherStation = function (raw) {
            var data = raw.map(function (item) { return item.municipalities; });
            console.log(data);
            return data[0];
        }
        return dataFactory;
    }])
    .factory('AgronomicFactory', ['$http', 'config', function ($http, config) {
        var dataFactory = {};
        /*
        * Method that request all agronomic information available from the forecast service
        */
        dataFactory.get = function () {
            if (dataFactory.raw == null) {
                dataFactory.raw = $http.get(config.api_fs + config.api_fs_agronomic);
            }
            return dataFactory.raw;
        }
        return dataFactory;
    }]);
