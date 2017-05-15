'use strict';

/**
 * @ngdoc service
 * @name ForecastApp.ForecastFactory
 * @description
 * # ForecastFactory
 * Factory in the ForecastApp.
 */
angular.module('ForecastApp')
    .factory('AgronomyFactory', function (ForecastApiFactory) {
        var dataFactory = {
            cache: true,
            db: ForecastApiFactory,
            format: "json"
        };

        /*
        * Method that request all agronomic information available from the forecast service
        * (bool) cultivar: True if request cultivar data, false if request soils data. It is only for the export data
        */
        dataFactory.get = function (cultivar) {
            dataFactory.db.init(dataFactory.cache, dataFactory.format);
            return dataFactory.db.getAgronomic(cultivar);
        }
        return dataFactory;
    });