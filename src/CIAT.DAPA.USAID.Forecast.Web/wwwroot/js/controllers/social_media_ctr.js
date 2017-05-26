'use strict';

/**
 * @ngdoc function
 * @name ForecastApp.controller:SocialMediaCtrl
 * @description
 * # SocialMediaCtrl
 * Controller of the ForecastApp
 */
angular.module('ForecastApp')
  .controller('SocialMediaCtrl', function ($scope, $rootScope, tools, AssistFactory) {
      $scope.type = tools.source();

      var parameters = { id: '', alt: '' };

      // Set the values for the assist (help button)
      if ($scope.type == 'climate')
          parameters = { id: 'forecast', alt: 'forecast' };
      else if ($scope.type == 'crop')
          parameters = { id: 'forecast', alt: 'yield' };
      else if ($scope.type == 'expert')
          parameters = { id: '', alt: '' };
      else
          parameters = { id: 'climate', alt: 'climate' };

      $rootScope.setAssistParameters = function (id, alt) {
          parameters.id = id;
          parameters.alt = alt;
      }

      $scope.assist = function () {         
          var data = AssistFactory.getByIdAlt(parameters.id, parameters.alt);          
          if (data != null)
              tools.show_assist(data.title, data.text, data.url);
          return false;
      }

  });