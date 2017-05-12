'use strict';

/**
 * @ngdoc service
 * @name ForecastApp.CultivarFactory
 * @description
 * # CultivarFactory
 * Cultivar in the ForecastApp.
 */
angular.module('ForecastApp')
    .factory('CultivarFactory', function (config) {
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
            return data.cultivars;
        }

        /*
        * Method that filter all cultivars available for the crop in the forecast by weather station
        * (object []) cultivars: Array of cultivar entity to check
        * (string) yield_forecast: Json with yield data of the weather station
        */
        dataFactory.getCultivarsAvailableForecast = function (cultivars, yield_forecast) {
            var data = [];
            for (var i = 0; i < cultivars.length; i++) {
                var cu = cultivars[i];
                var filtered = yield_forecast.yield.filter(function (item) { return item.cultivar === cu.id; });
                if (filtered.length >= 1)
                    data.push(cu);
            }
            return data;
        }

        /*
        * Method that filter all cultivars of the crop
        * (object) raw: Json with all agronomic data
        * (string) crop: Crop's name
        * (bool) national: True for national cultivar, false for imported cultivar
        */
        dataFactory.getByCropNational = function (raw, crop, national) {
            var data = raw.filter(function (item) {
                return item.cp_name.toLowerCase() === crop.toLowerCase();
            })[0];
            var cultivars = data.cultivars.filter(function (item) {
                return item.national == national;
            });
            return cultivars;
        }

        return dataFactory;
    });