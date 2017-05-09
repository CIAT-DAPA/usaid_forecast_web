'use strict';

/**
 * @ngdoc service
 * @name ForecastApp.WeatherStationFactory
 * @description
 * # WeatherStationFactory
 * Weather Station in the ForecastApp.
 */
angular.module('ForecastApp')
    .factory('WeatherStationFactory', function ($q, config, GeographicFactory) {
        var dataFactory = { cache: true };

        /*
       * Method that filter all weather stations by their municipality
       * (string) municipality: Name of the municipality
       */
        dataFactory.getByMunicipality = function (municipality) {
            var defer = $q.defer();
            GeographicFactory.cache = dataFactory.cache;

            GeographicFactory.get().then(
                 function (result) {
                     var raw = result.data;

                     var data = raw.filter(function (item) {
                         var municipalities = item.municipalities.filter(function (item2) { return item2.name === municipality; });
                         return municipalities.length > 0;
                     });
                     if (data == null)
                         defer.resolve(null);
                     // Map to get only weather station
                     var ws = data.map(function (item) {
                         var municipalities = item.municipalities.filter(function (item2) { return item2.name === municipality; });
                         return municipalities[0].weather_stations[0];
                     });
                     defer.resolve(ws[0]);

                 }, function (err) {
                     console.log(err);
                 });

            return defer.promise;
        }

        /*
        * Method that filter all ranges from a weather station
        * (object) ws: Json with data from weather station
        * (string) crop: Id crop
        */
        dataFactory.getRanges = function (ws, crop) {
            var answer = { labels: [], treashold: [] };
            var data = ws.ranges.filter(function (item) {
                return item.crop_name.toLowerCase() === crop.toLowerCase();
            });
            for (var i = 0; i < data.length; i++) {
                answer.labels.push(data[i].label);
                answer.treashold.push(data[i].upper);
            }
            return answer;
        }
        return dataFactory;





        /*
        * Method that filter all municipalities available
        */
        dataFactory.listAll = function () {
            var defer = $q.defer();
            GeographicFactory.cache = dataFactory.cache;

            GeographicFactory.get().then(
                function (raw) {
                    var data = raw.map(function (item) {
                        return item.municipalities;
                    });
                    var m = [];
                    for (var i = 0; i < data.length; i++)
                        for (var j = 0; j < data[i].length; j++)
                            m.push(data[i][j]);
                    defer.resolve(m);
                }, function (err) {
                    console.log(err);
                });

            return defer.promise;
        }

        /*
        * Method that filter the municipalities by their ids
        * (string[]) ids: Id of the municipalities
        */
        dataFactory.listByIds = function (ids) {
            var defer = $q.defer();

            dataFactory.listAll().then(
                function (raw) {
                    var data = raw.map(function (item) {
                        return item.municipalities;
                    });
                    var m = [];
                    for (var i = 0; i < data.length; i++)
                        for (var j = 0; j < data[i].length; j++)
                            if (ids.includes(data[i][j].id))
                                m.push(data[i][j]);
                    defer.resolve(m);
                },
                function (err) {
                    console.log(err);
                });
        }

        return dataFactory;
    });