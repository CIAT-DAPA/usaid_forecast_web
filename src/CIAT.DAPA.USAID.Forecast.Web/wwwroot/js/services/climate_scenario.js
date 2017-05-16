'use strict';

/**
 * @ngdoc service
 * @name ForecastApp.ClimateScenarioFactory
 * @description
 * # ClimateScenarioFactory
 * Scenarios Forecast in the ForecastApp.
 */
angular.module('ForecastApp')
    .factory('ClimateScenarioFactory', function ($q, config, ForecastApiFactory) {
        var dataFactory = {
            cache: true,
            db: ForecastApiFactory,
            format: "json"
        };

        /*
        * Method that request the last forecast to the web api
        * (string) ws: Id weather station
        */
        dataFactory.get = function (ws) {
            dataFactory.db.init(dataFactory.cache, dataFactory.format);
            return dataFactory.db.getClimate(ws, 'false');
        }

        /*
        * Method that filter all climate data from forecast of the weather station
        * (string) ws: Id of the weather station
        */
        dataFactory.getByWeatherStation = function (ws) {
            var defer = $q.defer();

            dataFactory.get(ws).then(
            function (result) {
                var raw = result.data;
                var data = raw.scenario.filter(function (item) {
                    return item.weather_station === ws;
                });
                defer.resolve(data);
            },
            function (err) { console.log(err); });

            return defer.promise;
        }

        /*
        * Method that return all probabilities from the forecast of the weather station
        * (string) ws: Id of the weather station
        */
        dataFactory.getScenarios = function (ws) {
            var defer = $q.defer();

            dataFactory.getByWeatherStation(ws).then(
            function (result) {
                var filtered = result;
                var data = filtered.map(function (item) {
                    var obj = [];
                    for (var i = 0; i < config.climate_vars_scenario.length; i++) {
                        var temp_m = config.climate_vars_scenario[i];
                        var scenarios = item.monthly_data.map(function (item2) {
                            var list_values = item2.data.filter(function (item3) { return item3.measure === temp_m.value; });
                            var values = list_values.map(function (item3) { return item3.value; });
                            return {
                                month: item2.month,
                                values: values
                            };
                        });
                        obj.push({
                            data: scenarios,
                            year: item.year,
                            measure: temp_m.value
                        });
                    }
                    return {                        
                        scenario: item.name,
                        content: obj
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