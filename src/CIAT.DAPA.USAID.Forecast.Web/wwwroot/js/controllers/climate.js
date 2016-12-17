'use strict';

/**
 * @ngdoc function
 * @name ForecastApp.controller:ClimateCtrl
 * @description
 * # ClimateCtrl
 * Controller of the ForecastApp
 */
angular.module('ForecastApp')
  .controller('ClimateCtrl', function ($scope, ForecastFactory, ClimateFactory) {
      var ws = "58504f4f006cb93ed40eec13";
      $scope.months = [];
      ForecastFactory.get().success(function (data) {          
          $scope.months = ClimateFactory.getProbabilities(data, ws, 'prec');
      }).error(function (error) {
          console.log(error);
      });;
  });