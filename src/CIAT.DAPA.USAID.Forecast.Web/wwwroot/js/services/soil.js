'use strict';

/**
 * @ngdoc service
 * @name ForecastApp.SoilFactory
 * @description
 * # SoilFactory
 * Soil in the ForecastApp.
 */
angular.module('ForecastApp')
    .factory('SoilFactory', ['config', function (config) {
        var dataFactory = {};
        /*
        * Method that filter all cultivars of the crop
        * (object) raw: Json with all agronomic data
        * (string) crop: Crop's name
        */
        dataFactory.getByCrop = function (raw, crop) {
            var data = raw.filter(function (item) {
                return item.cp_name.toLowerCase() === crop.toLowerCase();
            })[0];
            return data.soils;
        }

        /*
        * Method that filter all cultivars available for the crop in the forecast by weather station
        * (object []) soils: Array of soils entity to check
        * (string) cultivar: Cultivar id
        * (string) yield_forecast: Json with yield data of the weather station
        */
        dataFactory.getSoilsAvailableForecast = function (soils, cultivar, yield_forecast) {
            var data = [];
            for (var i = 0; i < soils.length; i++) {
                var so = soils[i];
                var filtered = yield_forecast.yield.filter(function (item) { return item.soil === so.id && item.cultivar === cultivar; });
                if (filtered.length >= 1)
                    data.push(so);
            }
            return data;
        }

        return dataFactory;
    }])