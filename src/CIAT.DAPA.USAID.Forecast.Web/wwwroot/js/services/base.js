'use strict';

/**
 * @ngdoc service
 * @name ForecastApp.ForecastFactory
 * @description
 * # ForecastFactory
 * Factory in the ForecastApp.
 */
angular.module('ForecastApp')
    .factory('GuildFactory', ['config', function (config) {
        var dataFactory = {};

        /*
        * Method that filter all vars of the crop
        * (string) crop: Crop's name
        */
        dataFactory.getByCrop = function (crop) {
            var data = config.yield_default_var.filter(function (item) { return item.crop === crop.toLowerCase(); });
            if (data == null || data.length < 1)
                return null
            return data[0].guild;
        }
        return dataFactory;
    }])
;
