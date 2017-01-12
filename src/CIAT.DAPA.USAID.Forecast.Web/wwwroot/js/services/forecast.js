﻿'use strict';

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
        /*
        * Method that request the last forecast to the web api
        */
        dataFactory.get = function () {
            if (dataFactory.raw == null) {
                dataFactory.raw = $http.get(config.api_fs + config.api_fs_forecast);
            }
            return dataFactory.raw;
        }
        return dataFactory;
    }])
    .factory('ClimateFactory', ['config', function (config) {

        var dataFactory = {};
        /*
        * Method that created a summary based in the probabilities
        * (object) p: Object with probabilities
        */
        function summaryProbabilities(p){
            var summary = '';
            if(p.lower > p.normal && p.lower > p.upper)
                summary = 'la probabilidad de precipitación estará por debajo de lo normal';
            else if (p.upper > p.normal && p.upper > p.lower)
                summary = 'la probabilidad de precipitación estará por encima de lo normal';
            else if (p.upper == p.normal && p.upper == p.lower)
                summary = 'las probabilidades de precipitación están muy similares';
            else 
                summary = 'la probabilidad de precipitación estará dentro de lo normal';
            return summary;
        }
        /*
        * Method that filter all climate data from forecast of the weather station
        * (object) raw: Json with all forecast
        * (string) ws: Id of the weather station
        */
        dataFactory.getByWeatherStation = function (raw, ws) {
            var data = raw.climate.filter(function (item) { return item.weather_station === ws; });
            return data[0];
        }
        /*
        * Method that return all probabilities from the forecast of the weather station
        * (object) raw: Json with all forecast
        * (string) ws: Id of the weather station
        * (string) measure: Name of measure climate
        */
        dataFactory.getProbabilities = function (raw, ws, measure) {
            var filtered = dataFactory.getByWeatherStation(raw, ws);
            var data = filtered.data.map(function (item) {
                var probabilities = item.probabilities.filter(function (item) { return item.measure === measure })[0];
                var p = new Array({ label: 'Arriba de lo normal', type:'upper', value: probabilities.upper },
                    { label: 'Normal', type: 'normal', value: probabilities.normal },
                    { label: 'Debajo de lo normal', type: 'lower', value: probabilities.lower });
                var obj = {
                    year: item.year,
                    month: item.month,
                    month_name: config.month_names[item.month-1],
                    probabilities: p,
                    summary: summaryProbabilities(probabilities)
                };
                return obj;
            });
            return data;
        }
        return dataFactory;
    }]);