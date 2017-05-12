'use strict';

/**
 * @ngdoc function
 * @name ForecastApp.CropHistoryCtrl
 * @description
 * # CropHistoryCtrl
 * Crop History of the ForecastApp
 */
angular.module('ForecastApp')
  .controller('CropHistoryCtrl', function ($rootScope, $scope, setup, tools,
                                    WeatherStationFactory,
                                    AgronomyFactory, CultivarFactory, SoilFactory,
                                    CropYieldHistoricalFactory, CropVarsFactory) {
      $scope.crop_name = tools.search('cultivo');
      // Get vars to show by crop
      $scope.municipality_name = tools.search('municipio');
      $scope.ws = null;
      // Historical data
      $scope.data_h_years = null;
      $scope.data_h = null;
      //$scope.historical_yield = {};
      $scope.cultivar_type = 'national';
      $scope.year_selected = null;
      // Agronomy data
      $scope.data_a = null;
      // Yield crop filtered by weather station
      $scope.yield_ws = null;
      // Yiel ranges for the weather station
      $scope.yield_ranges = null;

      $scope.loaded = false;
      $scope.search_historical = fixed_data_historical;

      function load_data() {
          // Load data from web web api
          AgronomyFactory.get().then(
          function (data_a) {
              $scope.data_a = data_a.data;
              // Get all geographic data able with information
              WeatherStationFactory.getByMunicipality($scope.municipality_name).then(
              function (data_ws) {
                  $scope.ws = data_ws;
                  // Get yield ranges
                  $scope.yield_ranges = WeatherStationFactory.getRanges($scope.ws, $scope.crop_name);
                  // Load the historical information
                  CropYieldHistoricalFactory.getYears($scope.ws.id).then(
                  function (result) {
                      $scope.data_h_years = result.data;
                      $scope.year_selected = $scope.data_h_years[0];
                      $scope.loaded = true;
                      fixed_data_historical();
                  },
                  function (error) { console.log(error); });
              },
              function (err) { console.log(err); });
          },
          function (err) { console.log(err); });
      }

      /*
       * Method that draw the yield historical data by source
       * (string) source: Indicates the source of the data
      */
      function fixed_data_historical() {
          // Load the historical information
          CropYieldHistoricalFactory.getByWeatherStationYear($scope.ws.id, $scope.year_selected).then(
          function (result) {
              var data_h = result.data;
              // Join all yield data in a single object                  
              $scope.data_h = CropYieldHistoricalFactory.consolidateHistoricalData(data_h);
              // Get cultivars (national or imported)
              var cultivars = CultivarsFactory.getByCropNational($scope.data_a, $scope.crop_name, $scope.cultivar_type === 'national');
              // Get the yield historical by 
              var data_cultivar = CropYieldHistoricalFactory.getByCultivars($scope.data_h, cultivars, $scope.crop_yield_var.name);
              // Draw calendar heatmap
              var base_chm = new Base('#cultivar_heatmap_model', data_cultivar);
              base_chm.setMargin(30, 30, 40, 10);
              base_chm.setDateNames(setup.getMonths(), setup.getDays());
              var c_heatmap = new CalendarHeatmap(base_chm, $scope.yield_ranges, $scope.year_selected);
              c_heatmap.render();
          },
          function (error) { console.log(error); });
      }

      $rootScope.drawFunction = function () {
          if (!$scope.loaded)
              load_data();
      }

  });