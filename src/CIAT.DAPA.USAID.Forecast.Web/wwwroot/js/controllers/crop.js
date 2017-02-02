'use strict';

/**
 * @ngdoc function
 * @name ForecastApp.controller:CropCtrl
 * @description
 * # CropCtrl
 * Controller of the ForecastApp
 */
angular.module('ForecastApp')
  .controller('CropCtrl', function ($scope, config, tools, HistoricalFactory, ForecastFactory, GeographicFactory, MunicipalityFactory, WeatherStationFactory, AgronomicFactory, CultivarsFactory, YieldForecastFactory, CropVarsFactory) {
      $scope.crop_name = tools.search('cultivo');
      // Get vars to show by crop
      $scope.crop_vars = CropVarsFactory.getVarsByCrop($scope.crop_name);
      $scope.crop_yield_var = CropVarsFactory.getDefaultVarByCrop($scope.crop_name);      
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
                      draw();
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

          // Forecast for every cultivar
          for (var i = 0; i < $scope.cultivars.length; i++) {
              try {
                  var cu = $scope.cultivars[i];
                  // Enable tabs for the variation graphic
                  // Add the event to show the tabs on click
                  $('#tabs_' + cu.id + ' a').click(function (e) {
                      e.preventDefault();
                      $(this).tab('show');
                  });

                  // Set the months for historical data
                  var h_month_start = parseInt($scope.gv_months[0]);
                  var h_month_end = parseInt($scope.gv_months[$scope.gv_months.length - 1]);

                  // Get data
                  var yield_ws = YieldForecastFactory.getByWeatherStation($scope.data_f, $scope.ws_entity.id);                  
                  var yield_cu = YieldForecastFactory.getByCultivarSoil(yield_ws.yield, cu.id, "5851ad2b07b6e43910c304b2");
                  // Draw the graphic
                  var base_c = new Base('#calendar_' + cu.id, yield_cu);
                  base_c.setMargin(10, 30, 10, 10);
                  var calendar = new Calendar(base_c, config.month_names, config.days_names, $scope.crop_yield_var.name);
                  calendar.render();

                  // The following cicle is used to draw the trend graphic
                  // by every variable of the yield crop
                  for (var j = 0; j < $scope.crop_vars.length; j++) {
                      var vr = $scope.crop_vars[j];
                      var vr_data = YieldForecastFactory.getByCultivarSoilMeasure(yield_cu, vr.name);
                      
                      // Draw the graphic
                      var base_t = new Base('#trend_'+ cu.id + '_' + vr.name, vr_data);
                      base_t.setMargin(10, 30, 10, 10);
                      var trend = new Trend(base_t);
                      trend.render();
                  }

              }
              catch (err) {
                  console.log(err);
              }
          }
      }

  });