'use strict';

/**
 * @ngdoc function
 * @name ForecastApp.controller:CropCtrl
 * @description
 * # CropCtrl
 * Controller of the ForecastApp
 */
angular.module('ForecastApp')
  .controller('CropCtrl', function ($scope, config, tools, HistoricalFactory, ForecastFactory, GeographicFactory, MunicipalityFactory, WeatherStationFactory, AgronomicFactory, CultivarsFactory, SoilFactory, YieldForecastFactory, CropVarsFactory) {
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
      $scope.soils = null;
      // Vars of the data
      // Data municipalities
      $scope.data_m = null;
      // Agronomic data
      $scope.data_a = null;
      // Historical data
      $scope.data_h = null;
      // Forecast data
      $scope.data_f = null;
      // Yield crop filtered by weather station
      $scope.yield_ws = null;

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
              // Load the list of the cultivars and soils from the agronomic configuration
              $scope.cultivars = CultivarsFactory.getByCrop($scope.data_a, $scope.crop_name);
              $scope.soils = SoilFactory.getByCrop($scope.data_a, $scope.crop_name);
              // Load the historical information
              HistoricalFactory.get($scope.ws_entity.id).success(function (data_h) {
                  $scope.data_h = data_h;
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
                      fixed_data();
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
       * Method that filter the data to show the information in the screen getted from the web api
      */
      function fixed_data() {          

          // Forecast for every cultivar
          for (var i = 0; i < $scope.cultivars.length; i++) {
              try {
                  var cu = $scope.cultivars[i];

                  // Filter the soils available for the cultivar
                  $scope.cultivars[i].soils = SoilFactory.getSoilsAvailableForecast($scope.soils, cu.id, $scope.yield_ws);
                                    
                  // Enable tabs for the variation graphic
                  // Add the event to show the tabs on click
                  $('#tabs_' + cu.id + ' a').click(function (e) {
                      e.preventDefault();
                      $(this).tab('show');
                  });

                  // Draw the graphic in the screen
                  draw(cu, $scope.cultivars[i].soils[0]);
              }
              catch (err) {
                  console.log(err);
              }
          }
          // Hide the content of the tabs variation
          $('.disable_tab').removeClass('active');
          $('.disable_tab').removeClass('in');
      }

      /*
       * Method that draw the graphics by a cultivar and soil
       * (object) cu: Cultivar entity
       * (object) so: Soil entity
      */
      function draw(cu, so) {
          // Set default color for all buttons of the soil by the cultivar
          $('#navbar_cultivar_' + cu.id + ' button').removeClass('btn-primary');
          $('#soil_'+ cu.id + '_' + so.id).addClass('btn-primary');

          // Get data
          var yield_cu = YieldForecastFactory.getByCultivarSoil($scope.yield_ws.yield, cu.id, so.id);
          
          // Draw the graphic
          var base_c = new Base('#calendar_' + cu.id, yield_cu);
          base_c.setMargin(10, 30, 10, 10);          
          var calendar = new Calendar(base_c, config.month_names, config.days_names, $scope.crop_yield_var.name, '#back_' + cu.id, '#forward_' + cu.id, '#current_month_' + cu.id);
          calendar.render();

          // Get the summary 
          var summary_cu_so = YieldForecastFactory.summaryCultivarSoil(yield_cu, $scope.crop_yield_var.name);
          $("#yield_" + cu.id + "_max_date").html(summary_cu_so.max.date.substring(0,10));
          $("#yield_" + cu.id + "_max_yield").html(summary_cu_so.max.value + ' ' + $scope.crop_yield_var.metric);
          $("#yield_" + cu.id + "_min_date").html(summary_cu_so.min.date.substring(0, 10));
          $("#yield_" + cu.id + "_min_yield").html(summary_cu_so.min.value + ' ' + $scope.crop_yield_var.metric);

          // The following cicle is used to draw the trend graphic
          // by every variable of the yield crop
          for (var j = 0; j < $scope.crop_vars.length; j++) {
              var vr = $scope.crop_vars[j];
              var vr_data = YieldForecastFactory.getByCultivarSoilMeasure(yield_cu, vr.name);
              
              // Draw the graphic
              var base_t = new Base('#trend_' + cu.id + '_' + vr.name, vr_data);
              base_t.setMargin(10, 50, 10, 20);
              base_t.setDateNames(config.month_names, config.days_names);
              var trend = new Trend(base_t);
              trend.render();

              // Get the summary by measure
              var summary_vr = YieldForecastFactory.summaryCultivarSoilMeasure(vr_data);
              $("#yield_" + vr.name + "_" + cu.id + "_max_date").html(summary_vr.max.date.substring(0, 10));
              $("#yield_" + vr.name + "_" + cu.id + "_max_yield").html(summary_vr.max.value + ' ' + vr.metric);
              $("#yield_" + vr.name + "_" + cu.id + "_min_date").html(summary_vr.min.date.substring(0, 10));
              $("#yield_" + vr.name + "_" + cu.id + "_min_yield").html(summary_vr.min.value + ' ' + vr.metric);
          }
      }

  });