'use strict';

/**
 * @ngdoc service
 * @name ForecastApp.ForecastFactory
 * @description
 * # ForecastFactory
 * Factory in the ForecastApp.
 */
angular.module('ForecastApp')
.factory('CropYieldForecastFactory', function ($q, config, ForecastApiFactory) {
    var dataFactory = {
        cache: true,
        db: ForecastApiFactory,
        format: "json"
    };

    /*
    * Method that request the last forecast to the web api
    * (string) ws: Id weater station
    */
    dataFactory.get = function (ws) {
        dataFactory.db.init(dataFactory.cache, dataFactory.format);
        return dataFactory.db.getForecastYield(ws);
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

    /*
    * Method that created a summary based in the forecast
    * (object) data: Forecast yield data filtered by cultivar and soil
    * (string) measure: Measure name
    */
    dataFactory.summaryCultivarSoil = function (data, measure) {
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
    * Method that created a summary based in the forecast
    * (object) data: Forecast yield data filtered by cultivar, soil and measure
    */
    dataFactory.summaryCultivarSoilMeasure = function (data) {
        var max = null;
        var min = null;
        for (var i = 0; i < data.length; i++) {
            if (max == null || max.value < data[i].data.sd)
                max = { date: data[i].date, value: data[i].data.sd };
            if (min == null || min.value > data[i].data.sd)
                min = { date: data[i].date, value: data[i].data.sd };
        }
        return { max: max, min: min };
    }

    return dataFactory;
});