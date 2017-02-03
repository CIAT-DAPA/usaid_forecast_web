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
        function summary(p){
            var summary = '';
            if(p.lower > p.normal && p.lower > p.upper)
                summary = 'la probabilidad de precipitación estará por debajo de lo normal';
            else if (p.upper > p.normal && p.upper > p.lower)
                summary = 'la probabilidad de precipitación estará por encima de lo normal';
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
                    summary: summary(probabilities)
                };
                return obj;
            });
            return data;
        }
        return dataFactory;
    }])
    .factory('YieldForecastFactory', ['config', function (config) {

        var dataFactory = {};
        /*
        * Method that created a summary based in the forecast
        * (object) data: Forecast yield data
        * (string) measure: Measure name
        */
        dataFactory.summaryCultivarSoil = function(data, measure) {
            var max = null;
            var min = null;
            for (var i = 0; i < data.length; i++) {
                var m = data[i].data.filter(function (item) { return item.measure === measure; })[0];
                if (max == null || max.value < m.avg)
                    max = { date: data[i].start, value: m.avg };
                if (min == null || min.value > m.avg)
                    min = { date: data[i].start, value: m.avg };
            }
            return { max: max, min: min };
        }
        /*
        * Method that filter all yield data from forecast of the weather station
        * (object) raw: Json with all forecast
        * (string) ws: Id of the weather station
        */
        dataFactory.getByWeatherStation = function (raw, ws) {
            var data = raw.yield.filter(function (item) { return item.weather_station === ws; });
            return data[0];
        }
        /*
        * Method that filter all yield data from forecast by cultivar and soil
        * (object) raw: Json with all forecast of the weather station
        * (string) cultivar: Id of the cultivar
        * (string) soil: Id of the cultivar
        */
        dataFactory.getByCultivarSoil = function (raw, cultivar, soil) {
            var data = raw.filter(function (item) { return item.cultivar === cultivar && item.soil === soil; });
            return data;
        }

        /*
        * Method that filter all yield data from forecast by cultivar and soil
        * (object) raw: Json with all forecast of the weather station, cultivar and soil (getByCultivarSoil)
        * (string) measure: Measure name
        */
        dataFactory.getByCultivarSoilMeasure = function (raw, measure) {
            var data = raw.map(function (item) {
                // Get measure' data
                var measure_filtered = item.data.filter(function (item2) {
                    return item2.measure === measure;
                });
                if (measure.length < 1)
                    return null;
                return {
                    date: item.start,
                    data: measure_filtered[0]
                }

            });
            return data;
        }

        return dataFactory;
    }]);
