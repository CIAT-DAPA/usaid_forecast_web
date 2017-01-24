'use strict';

/**
 * @ngdoc function
 * @name ForecastApp.controller:CropCtrl
 * @description
 * # CropCtrl
 * Controller of the ForecastApp
 */
angular.module('ForecastApp')
  .controller('CropCtrl', function ($scope, config, tools, HistoricalFactory, ClimatologyFactory, HistoricalClimateFactory, ForecastFactory, ClimateFactory, GeographicFactory, MunicipalityFactory, WeatherStationFactory, AgronomicFactory, CultivarsFactory) {
      $scope.crop_name = tools.search('cultivo');
      // Get the municipality from the url
      $scope.municipality_name = tools.search('municipio');
      $scope.municipalities = [];
      $scope.ws_entity = null;
      $scope.gv_months = $("#gv_months").val().split(',');
      $scope.gv_municipalities = $("#gv_municipalities").val().split(',');
      $scope.period_start = null;
      $scope.period_end = null;
      $scope.cultivars = null;
      // Vars of the data
      // Data municipalities
      $scope.data_m = null;
      // Agronomic data
      $scope.data_a = null;
      // Historical data
      $scope.data_h = null;
      // Forecast data
      $scope.data_f = null;

      // Load data from web web api
      // Get all geographic data able with information
      GeographicFactory.get().success(function (data_m) {
          $scope.data_m = data_m;
          // List all municipalities
          $scope.municipalities = MunicipalityFactory.listByIds(data_m, $scope.gv_municipalities);
          // Search the weather station
          $scope.ws_entity = WeatherStationFactory.getByMunicipality(data_m, $scope.municipality_name);
          // Load the agronomic information
          AgronomicFactory.get().success(function (data_a) {
              $scope.data_a = data_a;
              $scope.cultivars = CultivarsFactory.getByCrop($scope.data_a, $scope.crop_name)
              // Load the historical information
              HistoricalFactory.get($scope.ws_entity.id).success(function (data_h) {
                  $scope.data_h = data_h;
                  // Load the Forecast information
                  ForecastFactory.get().success(function (data_f) {
                      $scope.data_f = data_f;
                      // Draw the graphics
                      //draw();
                  }).error(function (error) {
                      console.log(error);
                  });
              }).error(function (error) {
                  console.log(error);
              });
          }).error(function (error) {
              console.log(error);
          });
      }).error(function (error) {
          console.log(error);
      });

      /*
       * Method that draw in screen the information getted from the web api
      */
      function draw() {
          // Historical data for every climate variable
          for (var i = 0; i < $scope.cultivars.length; i++) {
              try {
                  var cu = $scope.cultivars[i];
                  // Set the months for historical data
                  var h_month_start = parseInt($scope.gv_months[0]);
                  var h_month_end = parseInt($scope.gv_months[$scope.gv_months.length - 1]);
                  cv.historical_months = config.month_names.slice(h_month_start - 1, h_month_end);

                  // Climatology

                  // Get data
                  var climatology = ClimatologyFactory.getMonthlyData($scope.data_h, $scope.ws_entity.id, $scope.gv_months, cv.value);
                  // Draw the graphic
                  var base_c = new Base('#' + cv.value + '_bar_climatology', climatology);
                  base_c.setMargin(10, 30, 10, 10);
                  var bar = new Bars(base_c);
                  bar.render();
                  var compute_c = ClimatologyFactory.summary(climatology);

                  cv.month_start = climatology[0].month_name;
                  cv.month_end = climatology[climatology.length - 1].month_name;
                  cv.max = compute_c.max;
                  cv.min = compute_c.min;
              }
              catch (err) {
                  console.log(err);
              }
          }
      }

  });