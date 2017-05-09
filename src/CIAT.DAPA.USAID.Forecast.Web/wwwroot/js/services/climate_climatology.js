'use strict';

/**
 * @ngdoc service
 * @name ForecastApp.ClimateClimatologyFactory
 * @description
 * # ClimateClimatologyFactory
 * Climate Climatology in the ForecastApp.
 */
angular.module('ForecastApp')
    .factory('ClimateClimatologyFactory', function ($q, config, ClimateHistoryFactory) {
        var dataFactory = { cache: true };

        /*
        * Method that filter all climate data from forecast of the weather station
        * (string) ws: Id of the weather station
        */
        dataFactory.getByWeatherStation = function (ws) {
            var defer = $q.defer();
            ClimateHistoryFactory.cache = dataFactory.cache;

            ClimateHistoryFactory.get(ws).then(
                function (raw) {
                    var data = raw.climatology.filter(function (item) { return item.weather_station === ws; });
                    defer.resolve(data[0]);
                },
                function (err) {
                    console.log(err);
                });

            return defer.promise;
        }

        /*
        * Method that return all probabilities from the forecast of the weather station
        * (string) ws: Id of the weather station
        * (int[]) months: Array of the months to filter
        * (string) measure: Name of measure climate
        */
        dataFactory.getMonthlyData = function (ws, months, measure) {
            var defer = $q.defer();

            dataFactory.getByWeatherStation(ws).then(
                function (filtered_ws) {
                    var filtered_monthly = filtered_ws.monthly_data.filter(function (item) { return months.includes(item.month.toString()); });
                    // Transform months (delete de first zero) if data doesn't have information
                    if (filtered_monthly.length < 1) {
                        months = months.map(function (m) { return m.startsWith('0') ? m.replace('0', '') : m; });
                        filtered_monthly = filtered_ws.monthly_data.filter(function (item) { return months.includes(item.month.toString()); });
                    }
                    var data = filtered_monthly.map(function (item) {
                        var monthly = item.data.filter(function (item2) { return item2.measure === measure })[0];
                        var obj = {
                            month: item.month,
                            month_name: config.month_names[item.month - 1],
                            value: monthly.value
                        };
                        return obj;
                    });
                    defer.resolve(data);
                },
                function (err) {
                    console.log(err);
                });

            return defer.promise;
        }

        /*
        * Method that created a summary based in the climatology
        * (object) data: Climatology object
        */
        dataFactory.summary = function (data) {
            var max = null;
            var min = null;
            for (var i = 0; i < data.length; i++) {
                if (max == null || max.value < data[i].value)
                    max = data[i];
                if (min == null || min.value > data[i].value)
                    min = data[i];
            }
            return { max: max, min: min };
        }
                
        return dataFactory;
    });