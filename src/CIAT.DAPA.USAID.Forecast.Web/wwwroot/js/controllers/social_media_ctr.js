'use strict';

/**
 * @ngdoc function
 * @name ForecastApp.controller:SocialMediaCtrl
 * @description
 * # SocialMediaCtrl
 * Controller of the ForecastApp
 */
angular.module('ForecastApp')
  .controller('SocialMediaCtrl', function ($scope, $rootScope, tools) {
      $scope.type = tools.source();

  });