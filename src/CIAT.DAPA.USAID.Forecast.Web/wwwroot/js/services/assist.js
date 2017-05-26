'use strict';

/**
 * @ngdoc service
 * @name ForecastApp.AssistFactory
 * @description
 * # AssistFactory
 * Factory in the ForecastApp.
 */
angular.module('ForecastApp')    
    .factory('AssistFactory', ['config', function (config) {
        var dataFactory = {};

        /*
        * Method that get the assit record
        * (string) id: Id assist record
        */
        dataFactory.getById = function (id) {
            var data = config.assist.filter(function (item) { return item.id === id; })
            return data[0];
        }

        /*
        * Method that get the assit record by its id and alt
        * (string) id: Id assist record
        * (string) alt: Text to search in the record
        */
        dataFactory.getByIdAlt = function (id, alt) {
            var data = config.assist.filter(function (item) { return item.id === id && item.alt === alt; });            
            return data.length > 0 ? data[0] : null;
        }

        return dataFactory;
    }]);