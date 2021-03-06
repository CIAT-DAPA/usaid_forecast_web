﻿'use strict';

/**
 * @ngdoc service
 * @name ForecastApp.GeographicFactory
 * @description
 * # GeographicFactory
 * Geographic in the ForecastApp.
 */
angular.module('ForecastApp')
    .factory('GeographicFactory', function (ForecastApiFactory) {
        var dataFactory = {
            cache: true,
            db: ForecastApiFactory,
            format: "json"
        };

        /*
        * Method that request all geographic information available from the forecast service
        */
        dataFactory.get = function () {
            dataFactory.db.init(dataFactory.cache, dataFactory.format);
            return dataFactory.db.getGeographic();
        }

        /*
        * Method that request all geographic information available from the forecast service by every crop
        */
        dataFactory.getByCrop = function () {
            dataFactory.db.init(dataFactory.cache, dataFactory.format);
            return dataFactory.db.getGeographicCrop();
        }

        return dataFactory;
    });