'use strict';

/**
 * @ngdoc service
 * @name ForecastApp.CropYieldHistoricalFactory
 * @description
 * # CropYieldHistoricalFactory
 * Crop Historical Yield in the ForecastApp.
 */
angular.module('ForecastApp')
    .factory('CropYieldHistoricalFactory', function ($q, config, ForecastApiFactory) {
        var dataFactory = {
            cache: true,
            db: ForecastApiFactory,
            format: "json"
        };

        /*
        * Method that request the years available with information for a weather station
        * (string) ws: String with the id of the weather stations splited by comma
        */
        dataFactory.getYears = function (ws) {
            dataFactory.db.init(dataFactory.cache, dataFactory.format);
            return dataFactory.db.getHistoricalYieldYears(ws);
        }

        /*
        * Method that request the years available with information for a weather station
        * (string) ws: String with the id of the weather stations splited by comma
        * (string) years: Concatenate string with years to search data
        */
        dataFactory.getByWeatherStationYear = function (ws, years) {
            dataFactory.db.init(dataFactory.cache, dataFactory.format);
            return dataFactory.db.getHistoricalYield(ws, years);
        }

        /*
        * Method that join all yield historical data
        * (object) raw: Json with all yield historical
        */
        dataFactory.consolidateHistoricalData = function (raw) {
            var data = null;
            for (var i = 0; i < raw.length; i++) {
                if (i == 0)
                    data = raw[0];
                else
                    data.yield = data.yield.concat(raw[i].yield);
            }
            return data;
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
            // Filter by cultivar
            var data = raw.yield.filter(function (item) {
                return cultivars.filter(function (item2) { return item2.id === item.cultivar; }).length > 0;
            });

            // This cicle acum data by th
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
            var answer = [];
            for (var i = 0; i < summary.length; i++) {
                summary[i].avg = summary[i].avg_acu / summary[i].count;
                // This cicle add the new dates
                for (var d = new Date(summary[i].start) ; d <= new Date(summary[i].end) ; d.setDate(d.getDate() + 1)) {
                    var obj_y = {
                        date: d.toISOString().slice(0, 10).replace(/-/g, "-"),
                        avg: summary[i].avg,
                    };
                    answer.push(obj_y);
                }
            }
            return answer;
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
    });
