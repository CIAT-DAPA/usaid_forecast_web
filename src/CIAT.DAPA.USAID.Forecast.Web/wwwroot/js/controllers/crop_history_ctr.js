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
                                    CropYieldForecastFactory, CropVarsFactory) {
      $scope.crop_name = tools.search('cultivo');
      // Get vars to show by crop
      $scope.crop_vars = CropVarsFactory.getVarsByCrop($scope.crop_name);
      $scope.crop_yield_var = CropVarsFactory.getDefaultVarByCrop($scope.crop_name);
      // Get the municipality from the url
      $scope.municipality_name = tools.search('municipio');
      $scope.municipalities = [];
      $scope.ws_entity = null;
      $scope.gv_months = $("#gv_months").val().split(',');
      $scope.cultivars = null;
      $scope.soils = null;
      // Vars of the data
      // Data municipalities
      $scope.data_m = null;
      // Agronomic data
      $scope.data_a = null;
      // Historical data
      $scope.data_h_years = null;
      $scope.data_h = null;
      $scope.historical_yield = {};
      $scope.historical_yield.model_type = 'national';
      // Forecast data
      $scope.data_f = null;
      // Yield crop filtered by weather station
      $scope.yield_ws = null;
      // Yiel ranges for the weather station
      $scope.yield_ranges = null;
      
      // Load data from web web api
      // Get all geographic data able with information
      GeographicFactory.get().success(function (data_m) {
          $scope.data_m = data_m;
          // List all municipalities
          $scope.municipalities = MunicipalityFactory.listByIds(data_m, $scope.gv_municipalities);
          // Search the weather station
          $scope.ws_entity = WeatherStationFactory.getByMunicipality(data_m, $scope.municipality_name);
          // Get yield ranges
          $scope.yield_ranges = WeatherStationFactory.getRanges($scope.ws_entity, $scope.crop_name);

          // Load the agronomic information
          AgronomicFactory.get().success(function (data_a) {
              $scope.data_a = data_a;
              // Load the list of the cultivars and soils from the agronomic configuration
              $scope.cultivars = CultivarsFactory.getByCrop($scope.data_a, $scope.crop_name);
              $scope.soils = SoilFactory.getByCrop($scope.data_a, $scope.crop_name);

              // Load the Forecast information
              ForecastFactory.get().success(function (data_f) {
                  $scope.data_f = data_f;
                  // Filter data for weather station
                  $scope.yield_ws = YieldForecastFactory.getByWeatherStation($scope.data_f, $scope.ws_entity.id);

                  $scope.cultivars = CultivarsFactory.getCultivarsAvailableForecast($scope.cultivars, $scope.yield_ws);
                  // Set the period of the forecast
                  var temp_date = $scope.gv_months[0].split('-');
                  $scope.period_start = config.month_names[parseInt(temp_date[1]) - 1] + ", " + temp_date[0];
                  temp_date = $scope.gv_months[1].split('-');
                  $scope.period_end = config.month_names[parseInt(temp_date[1]) - 1] + ", " + temp_date[0];
                  // Draw the graphics

                  for (var i = 0; i < $scope.cultivars.length; i++) {
                      var cu = $scope.cultivars[i];
                      // Filter the soils available for the cultivar
                      if ($scope.cultivars[i].soils == undefined || $scope.cultivars[i].soils == null)
                          $scope.cultivars[i].soils = SoilFactory.getSoilsAvailableForecast($scope.soils, cu.id, $scope.yield_ws);
                  }

                  //fixed_data_forecast(0);
              }).error(function (error) {
                  console.log(error);
              });


              // Load the historical information
              HistoricalYieldFactory.getYears($scope.ws_entity.id).success(function (data_h_years) {
                  $scope.data_h_years = data_h_years;
                  $scope.historical_yield.model = $scope.data_h_years[0];
                  //fixed_data_historical('model');
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
       * Method that draw the yield historical data by source
       * (string) source: Indicates the source of the data
      */
      function fixed_data_historical(source) {
          if (source === 'model') {
              // Load the historical information
              HistoricalYieldFactory.getByWeatherStationYear($scope.ws_entity.id, $scope.historical_yield.model).success(function (data_h) {
                  // Join all yield data in a single object                  
                  $scope.data_h = HistoricalYieldFactory.consolidateHistoricalData(data_h);
                  // Get cultivars (national or imported)
                  var cultivars = CultivarsFactory.getByCropNational($scope.data_a, $scope.crop_name, $scope.historical_yield.model_type === 'national');
                  // Get the yield historical by 
                  var data_cultivar = HistoricalYieldFactory.getByCultivars($scope.data_h, cultivars, $scope.crop_yield_var.name);
                  // Draw calendar heatmap
                  var base_chm = new Base('#cultivar_heatmap_model', data_cultivar);
                  base_chm.setMargin(30, 30, 40, 10);
                  base_chm.setDateNames(config.month_names, config.days_names);
                  var c_heatmap = new CalendarHeatmap(base_chm, $scope.yield_ranges, $scope.historical_yield.model);
                  c_heatmap.render();
              }).error(function (error) {
                  console.log(error);
              });
          }
      }

      $scope.search_historical = fixed_data_historical;
            
      $rootScope.drawFunction = function () {
          
      }

  });