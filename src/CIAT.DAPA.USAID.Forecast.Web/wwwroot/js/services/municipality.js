'use strict';

/**
 * @ngdoc service
 * @name ForecastApp.MunicipalityFactory
 * @description
 * # MunicipalityFactory
 * Municipality in the ForecastApp.
 */
angular.module('ForecastApp')
    .factory('MunicipalityFactory', function ($q, config, GeographicFactory) {
        var dataFactory = { cache: true };

        /*
        * Method that filter all municipalities available
        */
        dataFactory.listAll = function () {
            var defer = $q.defer();
            GeographicFactory.cache = dataFactory.cache;

            GeographicFactory.get().then(
                function (result) {
                    var raw = result.data;
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
                function (result) {
                    var raw = result.data;
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

        /*
        * Method that filter the municipalities by their name
        * (string) name: Municipality name
        */
        dataFactory.listByName = function (name) {
            var defer = $q.defer();

            dataFactory.listAll().then(
                function (result) {
                    var raw = result.data;
                    var data = raw.map(function (item) {
                        return item.municipalities;
                    });
                    var m = [];
                    for (var i = 0; i < data.length; i++)
                        for (var j = 0; j < data[i].length; j++)
                            if (name === data[i][j].name)
                                m.push(data[i][j]);
                    defer.resolve(m);
                },
                function (err) {
                    console.log(err);
                });
        }

        return dataFactory;
    });