'use strict';

/**
 * @ngdoc service
 * @name ForecastApp.ClimateClimatologyFactory
 * @description
 * # ClimateClimatologyFactory
 * Climate Climatology in the ForecastApp.
 */
angular.module('ForecastApp')
    .factory('ClimateClimatologyFactory', function ($q, config, ForecastApiFactory) {
        var dataFactory = {
            cache: true,
            db: ForecastApiFactory,
            format: "json"
        };

        /*
        * Method that filter all climate data from forecast of the weather station
        * (string) ws: Id of the weather station
        */
        dataFactory.getByWeatherStation = function (ws) {
            var defer = $q.defer();

            dataFactory.db.init(dataFactory.cache, dataFactory.format);
            dataFactory.db.getHistoricalClimatology(ws).then(
            function (result) {
                var raw = result.data;
                var data = raw.filter(function (item) { return item.weather_station === ws; });
                defer.resolve(data[0]);
            },
            function (err) { console.log(err); });

            return defer.promise;
        }

        /*
        * Method that return one climate var of the climatology by weather station
        * (string) ws: Id of the weather station
        * (int[]) months: Array of the months to filter
        * (string) measure: Name of measure climate
        */
        dataFactory.getMonthlyData = function (ws, months, measure) {
            var defer = $q.defer();

            dataFactory.getByWeatherStation(ws).then(
            function (filtered_ws) {
                var filtered_monthly = filtered_ws.monthly_data.filter(function (item) {
                    var tm = item.month.toString().length == 1 ? '0' + item.month.toString() : item.month.toString();
                    return months.includes(tm);
                });
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
            function (err) { console.log(err); });

            return defer.promise;
        }

        /*
        * Method that return all climate vars of the climatology by weather station
        * (string) ws: Id of the weather station
        * (string[]) months: Array of the months to filter
        */
        dataFactory.getMonthly = function (ws, months) {
            var defer = $q.defer();

            dataFactory.getByWeatherStation(ws).then(
            function (filtered_ws) {

                var filtered_monthly = filtered_ws.monthly_data.filter(function (item) {
                    var tm = item.month.toString().length == 1 ? '0' + item.month.toString() : item.month.toString();
                    return months.includes(tm);
                });
                var data = filtered_monthly.map(function (item) {
                    return item.data.map(function (item2) {
                        return {
                            month: item.month,
                            month_name: config.month_names[item.month - 1],
                            // Transform solar radiation
                            value: (item2.measure === 'sol_rad' ? item2.value / 0.041868 : item2.value),
                            measure: item2.measure
                        }
                    });
                });
                defer.resolve(data);
            },
            function (err) { console.log(err); });

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