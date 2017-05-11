'use strict';

/**
 * @ngdoc service
 * @name ForecastApp.CropVarsFactory
 * @description
 * # CropVarsFactory
 * Crop Vars in the ForecastApp.
 */
angular.module('ForecastApp')
    .factory('CropVarsFactory', function (config) {
        var dataFactory = {};

        /*
        * Method that filter all vars of the crop
        * (string) crop: Crop's name
        */
        dataFactory.getVarsByCrop = function (crop) {
            var data = config.yield_default_var.filter(function (item) { return item.crop === crop.toLowerCase(); });
            if (data == null || data.length < 1)
                return null
            return data[0].vars;
        }

        /*
        * Method that get the default var by cultivars of the crop
        * (string) crop: Crop's name
        */
        dataFactory.getDefaultVarByCrop = function (crop) {
            var data = dataFactory.getVarsByCrop(crop);
            var row = data.filter(function (item) { return item.default; });
            return row[0];
        }

        return dataFactory;
    });