'use strict';

/**
 * @ngdoc service
 * @name ForecastApp.HistoricalFactory
 * @description
 * # HistoricalFactory
 * Factory in the ForecastApp.
 */
angular.module('ForecastApp')
    .factory('HistoricalFactory', ['$http', 'config', function ($http, config) {
        var dataFactory = {};

        /*
        * Method that request the climatology to the web api
        * (string) ws: String with the id of the weather stations splited by comma
        */
        dataFactory.get = function (ws) {
            if (dataFactory.raw == null) {
                dataFactory.raw = $http.get(config.api_historical + ws);
            }
            return dataFactory.raw;
        }
        return dataFactory;
    }])
    .factory('ClimatologyFactory', ['config', function (config) {

        var dataFactory = {};
        /*
        * Method that filter all climate data from forecast of the weather station
        * (object) raw: Json with all forecast
        * (string) ws: Id of the weather station
        */
        dataFactory.getByWeatherStation = function (raw, ws) {
            var data = raw.climatology.filter(function (item) { return item.weather_station === ws; });
            return data[0];
        }
        /*
        * Method that return all probabilities from the forecast of the weather station
        * (object) raw: Json with all forecast
        * (string) ws: Id of the weather station
        * (int[]) months: Array of the months to filter
        * (string) measure: Name of measure climate
        */
        dataFactory.getMonthlyData = function (raw, ws, months, measure) {
            var filtered_ws = dataFactory.getByWeatherStation(raw, ws);
            var filtered_monthly = filtered_ws.monthly_data.filter(function (item) { return months.includes(item.month.toString()); });
            var data = filtered_monthly.map(function (item) {
                var monthly = item.data.filter(function (item2) { return item2.measure === measure })[0];
                var obj = {
                    month: item.month,
                    month_name: config.month_names[item.month - 1],
                    value: monthly.value
                };
                return obj;
            });
            return data;
        }
        return dataFactory;
    }])
    .factory('HistoricalClimateFactory', ['config', function (config) {

        var dataFactory = {};
        /*
        * Method that filter all climate data from forecast of the weather station
        * (object) raw: Json with all forecast
        * (string) ws: Id of the weather station
        */
        dataFactory.getByWeatherStation = function (raw, ws) {
            var data = raw.climate.filter(function (item) { return item.weather_station === ws; });
            return data;
        }

        /*
        * Method that return all probabilities from the forecast of the weather station
        * (object) raw: Json with all forecast
        * (string) ws: Id of the weather station
        * (int) month: Month to get data
        * (string) measure: Name of measure climate
        */
        dataFactory.getData = function (raw, ws, month, measure) {
            var filtered_ws = dataFactory.getByWeatherStation(raw, ws);
            var data = filtered_ws.map(function (item) {
                var filtered_monthly = item.monthly_data.filter(function (item2) { return month.toString() === item2.month.toString(); });
                var data2 = filtered_monthly.map(function (item2) {
                    var monthly = item2.data.filter(function (item3) { return item3.measure === measure })[0];
                    var obj = {
                        month: item2.month,
                        month_name: config.month_names[item2.month - 1],
                        value: monthly.value
                    };
                    return obj;
                });
                var obj = {
                    year: item.year,
                    month: data2[0].month,
                    month_name: data2[0].month_name,
                    value: data2[0].value,
                    date: new Date(item.year,1,1)
                    //date: new Date(item.year, data2[0].month, 1)
                };
                return obj;
            });
            return data;
        }
        return dataFactory;
    }]);
