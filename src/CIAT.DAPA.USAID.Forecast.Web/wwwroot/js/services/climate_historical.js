'use strict';

/**
 * @ngdoc service
 * @name ForecastApp.ClimateHistoricalFactory
 * @description
 * # ClimateHistoricalFactory
 * Climate Historical in the ForecastApp.
 */
angular.module('ForecastApp')
    .factory('ClimateHistoricalFactory', function ($q, config, ClimateHistoryFactory) {
        var dataFactory = { cache: true };

        /*
        * Method that filter all climate data from forecast of the weather station
        * (string) ws: Id of the weather station
        */
        dataFactory.getByWeatherStation = function (ws) {
            var defer = $q.defer();
            ClimateHistoryFactory.cache = dataFactory.cache;

            ClimateHistoryFactory.get(ws).then(
                function (result) {
                    var raw = result.data;
                    var data = raw.climate.filter(function (item) { return item.weather_station === ws; });
                    defer.resolve(data);
                },
                function (err) {
                    console.log(err);
                });

            return defer.promise;
        }

        /*
        * Method that return all probabilities from the forecast of the weather station
        * (string) ws: Id of the weather station
        * (int) month: Month to get data
        * (string) measure: Name of measure climate
        */
        dataFactory.getData = function (ws, month, measure) {
            var defer = $q.defer();

            dataFactory.getByWeatherStation(raw, ws).then(
                function (result) {
                    var filtered_ws = result.data;
                    var data = filtered_ws.map(function (item) {
                        var filtered_monthly = item.monthly_data.filter(function (item2) { return month.toString() === item2.month.toString(); });
                        var data2 = filtered_monthly.map(function (item2) {
                            var monthly = item2.data.filter(function (item3) { return item3.measure === measure })[0];
                            if (monthly == null)
                                return null;
                            else
                                return {
                                    month: item2.month,
                                    month_name: config.month_names[item2.month - 1],
                                    value: monthly.value
                                };

                        });
                        if (data2[0] == null)
                            return null;
                        else
                            return {
                                year: item.year,
                                month: data2[0].month,
                                month_name: data2[0].month_name,
                                value: data2[0].value,
                                date: new Date(item.year, 1, 1)
                            };
                    });
                    defer.resolve(data.filter(function (item) { return item != null; }));
                },
                function(err){
                    console.log(err);
                });

            return defer.promise;
        };

        /*
        * Method that created a summary based in the historical climate data
        * (object) data: Historical climate object
        * (object) climatology: Climatology object
        */
        dataFactory.summary = function (data, climatology) {
            var count_upper = data.filter(function (item) {
                var cl = climatology.filter(function (item2) { return item.measure === item2.measure; });
                return item.value > cl[0].value
            }).length;
            var count_lower = data.filter(function (item) {
                var cl = climatology.filter(function (item2) { return item.measure === item2.measure; });
                return item.value < cl[0].value
            }).length;
            return { upper: count_upper, lower: count_lower };
        }
        return dataFactory;
    });