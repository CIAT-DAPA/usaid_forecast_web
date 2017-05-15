'use strict';

/**
 * @ngdoc service
 * @name ForecastApp.ClimateForecastFactory
 * @description
 * # ClimateForecastFactory
 * Climate Forecast in the ForecastApp.
 */
angular.module('ForecastApp')
    .factory('ClimateForecastFactory', function ($q, config, ForecastApiFactory) {
        var dataFactory = {
            cache: true,
            db: ForecastApiFactory,
            format: "json"
        };

        /*
        * Method that request the last forecast to the web api
        * (string) ws: Id weather station
        * (bool) probabilities: True for probabilities, false for scenarios
        */
        dataFactory.get = function (ws, probabilities) {
            dataFactory.db.init(dataFactory.cache, dataFactory.format);
            return dataFactory.db.getClimate(ws, probabilities);
        }

        /*
        * Method that filter all climate data from forecast of the weather station
        * (string) ws: Id of the weather station
        * (bool) probabilities: True for probabilities, false for scenarios
        */
        dataFactory.getByWeatherStation = function (ws, probabilities) {
            var defer = $q.defer();

            dataFactory.get(ws, probabilities).then(
            function (result) {                
                var raw = result.data;
                var data = raw.climate.filter(function (item) {
                    return item.weather_station === ws;
                });
                defer.resolve(data[0]);
            },
            function (err) { console.log(err); });

            return defer.promise;
        }

        /*
        * Method that return all probabilities from the forecast of the weather station
        * (string) ws: Id of the weather station
        * (string) measure: Name of measure climate
        * (bool) probabilities: True for probabilities, false for scenarios
        */
        dataFactory.getProbabilities = function (ws, measure, probabilities) {
            var defer = $q.defer();

            dataFactory.getByWeatherStation(ws, probabilities).then(
            function (result) {
                var filtered = result.data;
                var data = filtered.map(function (item) {
                    var probabilities = item.probabilities.filter(function (item) { return item.measure === measure })[0];
                    var p = new Array({ label: 'Arriba de lo normal', type: 'upper', value: probabilities.upper },
                        { label: 'Normal', type: 'normal', value: probabilities.normal },
                        { label: 'Debajo de lo normal', type: 'lower', value: probabilities.lower });
                    return {
                        year: item.year,
                        month: item.month,
                        month_name: config.month_names[item.month - 1],
                        probabilities: p,
                        raw: probabilities
                    };
                });
                defer.resolve(data);
            },
            function (err) { console.log(err); });

            return defer.promise;
        }

        /*
        * Method that created a summary based in the probabilities
        * (object) p: Object with probabilities
        */
        dataFactory.summary = function (p) {
            var summary = '';
            if (p.lower > p.normal && p.lower > p.upper)
                summary = 'la probabilidad de precipitación estará por debajo de lo normal';
            else if (p.upper > p.normal && p.upper > p.lower)
                summary = 'la probabilidad de precipitación estará por encima de lo normal';
            else
                summary = 'la probabilidad de precipitación estará dentro de lo normal';
            return summary;
        }

        return dataFactory;
    });