'use strict';

/**
 * @ngdoc function
 * @name ForecastApp.controller:ClimatologyCtrl
 * @description
 * # ClimatologyCtrl
 * Controller of the ForecastApp
 */
angular.module('ForecastApp')
  .controller('ClimatologyCtrl', function ($scope, HistoricalFactory, ClimatologyFactory) {
      var ws = "58504f4f006cb93ed40eec13";
      var months = $("#gv_months").val().split(',');
      HistoricalFactory.get(ws).success(function (data) {
          var climatology = ClimatologyFactory.getMonthlyData(data, ws, months, 'prec');          
          var base = new Base('#bar_climatology');
          var bar = new Bars(base);
          bar.render(climatology);
      }).error(function (error) {
          console.log(error);
      });
  });