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
    }]);
