'use strict';

/**
 * @ngdoc service
 * @name ForecastApp.TutorialFactory
 * @description
 * # TutorialFactory
 * Factory in the ForecastApp.
 */
angular.module('ForecastApp')
    .factory('TutorialFactory', ['config', '$cookies', function (config, $cookies) {
        var dataFactory = {};

        /*
        * Method to get if the tutorial should be deployed
        */
        dataFactory.show_tutorial = function () {            
            var show_tutorial = $cookies.get('show_tutorial');
            if (show_tutorial == undefined || show_tutorial == null) {
                show_tutorial = true;
                $cookies.put('show_tutorial', show_tutorial);
            }
            show_tutorial = show_tutorial === 'true';
            return show_tutorial;
        }

        dataFactory.setShowTutorial = function (value) {
            $cookies.put('show_tutorial', value);
        }

        return dataFactory;
    }]);