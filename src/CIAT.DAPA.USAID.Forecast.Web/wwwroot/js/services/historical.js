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
         * Method that return the url to get data geographic
        */
        dataFactory.getUrl = function (ws) {
            return config.api_fs + config.api_fs_historical + ws;
        }

        /*
        * Method that request the climatology to the web api
        * (string) ws: String with the id of the weather stations splited by comma
        */
        dataFactory.get = function (ws) {
            if (dataFactory.raw == null) {
                dataFactory.raw = $http.get(dataFactory.getUrl(ws));
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
            return data;
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
            return data.filter(function (item) { return item != null; });
        };

        /*
        * Method that created a summary based in the historical climate data
        * (object) data: Historical climate object
        * (float) cl: Climatology value
        */
        dataFactory.summary = function (data, cl) {
            var count_upper = data.filter(function (item) { return item.value > cl }).length;
            var count_lower = data.filter(function (item) { return item.value < cl }).length;
            var count_cl = data.filter(function (item) { return item.value == cl }).length;
            return { upper: count_upper, lower: count_lower, equals: count_cl };
        }
        return dataFactory;
    }])
    .factory('HistoricalYieldFactory', ['$http', 'config', function ($http, config) {

        var dataFactory = {};

        /*
         * Method that return the url to get yield historical years
         * (string) ws: Concatenate string with ids of the weather stations
        */
        dataFactory.getUrlYears = function (ws) {
            return config.api_fs + config.api_fs_historical_yield_years + ws;
        }

        /*
        * Method that request the years available with information for a weather station
        * (string) ws: String with the id of the weather stations splited by comma
        */
        dataFactory.getYears = function (ws) {
            if (dataFactory.raw_y == null) {
                dataFactory.raw_y = $http.get(dataFactory.getUrlYears(ws));
            }
            return dataFactory.raw_y;
        }

        /*
         * Method that return the url to get yield historical
         * (string) ws: Concatenate string with ids of the weather stations
         * (string) years: Concatenate string with years to search data
        */
        dataFactory.getUrl = function (ws, years) {
            return config.api_fs + config.api_fs_historical_yield + ws + "&years=" + years;
        }

        /*
        * Method that request the years available with information for a weather station
        * (string) ws: String with the id of the weather stations splited by comma
        * (string) years: Concatenate string with years to search data
        */
        dataFactory.getByWeatherStationYear = function (ws, years) {
            if (dataFactory.raw == null) {
                dataFactory.raw = $http.get(dataFactory.getUrl(ws, years));
            }
            return dataFactory.raw;
        }

        /*
        * Method that filter the cultivars and make a summary by specific cultivar
        * (object) raw: Json with all yield historical
        * (object[]) cultivars: Cultivars list to summary data
        * (string) measure: Cultivars list to summary data
        */
        dataFactory.getByCultivars = function (raw, cultivars, measure) {
            var summary = [];
            var j = 0;
            var yield_row = null;

            // Get only yield data
            var yield_h = raw.map(function (item) { return item.yield; });
            // Filter by cultivar
            var data = yield_h[0].filter(function (item) {
                return cultivars.filter(function (item2) { return item2.id === item.cultivar }).length > 0;
            });

            for (var i = 0; i < data.length; i++) {
                j = indexByDate(summary, data[i].start);
                // Get yield var
                yield_row = data[i].data.filter(function (item) { return item.measure === measure; });                
                // Create a new row by every start date
                if (j < 0 && yield_row.length > 0) {
                    var obj = {
                        start: data[i].start,
                        end: data[i].end,
                        avg_acu: yield_row[0].avg,
                        count: 1
                    };
                    summary.push(obj);
                }
                else if (yield_row.length > 0) {
                    summary[j].avg_acu += yield_row[0].avg;
                    summary[j].count += 1;
                }
            }
            // Calculate the avg for every date
            for (var i = 0; i < summary.length; i++)
                summary[i].avg = summary[i].avg_acu / summary[i].count;
            return summary;
        }

        /*
        * Method that search a yield crop data by its start date.
        * If the date doesn't exist in the array, it will return -1
        * (object[]) data_yield: array of yield crop
        * (string) date: Date to search in data yield
        */
        function indexByDate(data_yield, date) {
            for (var i = 0; i < data_yield.length; i++)
                if (data_yield[i].start === date)
                    return i;
            return -1;
        }

        return dataFactory;
    }])
;
