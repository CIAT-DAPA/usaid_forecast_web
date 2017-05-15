'use strict';

/**
 * @ngdoc function
 * @name ForecastApp.controller:CropCtrl
 * @description
 * # CropCtrl
 * Controller of the ForecastApp
 */
angular.module('ForecastApp')
  .controller('CropForecastCtrl', function ($rootScope, $scope, setup, tools,
                                    WeatherStationFactory,
                                    AgronomyFactory, CultivarFactory, SoilFactory,
                                    CropYieldForecastFactory, CropVarsFactory) {
      // Get the municipality from the url
      $scope.municipality_name = tools.search('municipio');
      $scope.crop_name = tools.search('cultivo');
      // Get vars to show by crop
      $scope.crop_vars = CropVarsFactory.getVarsByCrop($scope.crop_name);
      $scope.crop_yield_var = CropVarsFactory.getDefaultVarByCrop($scope.crop_name);
      // Get the guild
      //$scope.guild = GuildFactory.getByCrop($scope.crop_name);

      $scope.municipalities = [];
      $scope.ws = null;
      $scope.gv_months = $("#gv_months").val().split(',');
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

      $scope.loaded = false;

      if ($scope.municipality_name != null && $scope.municipality_name !== '')
          load_data();

      function load_data() {
          // Load data from web web api
          // Load the agronomic information
          AgronomyFactory.get(true).then(
          function (data_a) {
              $scope.data_a = data_a.data;
              // Load the list of the cultivars and soils from the agronomic configuration
              var cultivars_temp = CultivarFactory.getByCrop($scope.data_a, $scope.crop_name);
              $scope.soils = SoilFactory.getByCrop($scope.data_a, $scope.crop_name);

              // Get all geographic data able with information
              WeatherStationFactory.getByMunicipality($scope.municipality_name).then(
              function (data_ws) {
                  $scope.ws = data_ws;
                  // Get yield ranges
                  $scope.yield_ranges = WeatherStationFactory.getRanges($scope.ws, $scope.crop_name);

                  // Load the Forecast information
                  CropYieldForecastFactory.get($scope.ws.id).then(
                  function (data_f) {
                      $scope.data_f = data_f.data;

                      // Filter data for weather station
                      $scope.yield_ws = CropYieldForecastFactory.getByWeatherStation($scope.data_f, $scope.ws.id);
                      $scope.cultivars = CultivarFactory.getCultivarsAvailableForecast(cultivars_temp, $scope.yield_ws);


                      // Set the period of the forecast
                      var temp_date = $scope.gv_months[0].split('-');
                      $scope.period_start = setup.getMonths()[parseInt(temp_date[1]) - 1] + ", " + temp_date[0];
                      temp_date = $scope.gv_months[1].split('-');
                      $scope.period_end = setup.getMonths()[parseInt(temp_date[1]) - 1] + ", " + temp_date[0];
                      // Draw the graphics

                      for (var i = 0; i < $scope.cultivars.length; i++) {
                          var cu = $scope.cultivars[i];
                          // Filter the soils available for the cultivar
                          if ($scope.cultivars[i].soils == undefined || $scope.cultivars[i].soils == null)
                              $scope.cultivars[i].soils = SoilFactory.getSoilsAvailableForecast($scope.soils, cu.id, $scope.yield_ws);
                      }
                  },
                  function (error) { console.log(error); });

              },
              function (error) { console.log(error); });
          },
          function (err) { console.log(err); });
      }

      $scope.collapsable = function (item, index) {
          var $this = $("#" + item).find(".blockTitle");
          if ($($this).hasClass('closed')) {
              $($this).parent().find('.blockTitle').removeClass('opened').addClass('closed');
              $($this).removeClass('closed').addClass('opened');
          } else {
              $($this).removeClass('opened').addClass('closed');
          }
          $($this).next().slideToggle('slow', function () {
              if ($($this).hasClass('opened')) {
                  $(".tab-content").find("div.tab-pane").addClass("active");
                  fixed_data_forecast(index);
              }
          });

      }

      $scope.drawForecast = draw_forecast;

      /*
       * Method that filter the data to show the information in the screen getted from the web api
      */
      function fixed_data_forecast(i) {

          var cu = $scope.cultivars[i];

          $scope.cultivars[i].soil_selected = $scope.cultivars[i].soils[0];
          draw_forecast(cu);
      }

      /*
       * Method that draw the graphics by a cultivar and soil of the forecast
       * (object) cu: Cultivar entity
      */
      function draw_forecast(cu) {
          var so = cu.soil_selected;
          $('#content_' + cu.id + ' div').addClass('active');
          $('#content_' + cu.id + ' div').addClass('in');

          // Enable tabs for the variation graphic
          // Add the event to show the tabs on click
          $('#tabs_' + cu.id + ' a').click(function (e) {
              e.preventDefault();
              $(this).tab('show');
          });

          // Get data
          var yield_cu = CropYieldForecastFactory.getByCultivarSoil($scope.yield_ws.yield, cu.id, so.id);

          // Draw the graphic
          var base_c = new Base('#calendar_' + cu.id, yield_cu);
          base_c.setMargin(10, 30, 10, 10);
          var calendar = new Calendar(base_c, setup.getMonths(), setup.getDays(), $scope.crop_yield_var.name, '#back_' + cu.id, '#forward_' + cu.id, '#current_month_' + cu.id, $scope.yield_ranges, 'c_alias_' + cu.id);
          calendar.render();

          // Get the summary 
          var float = setup.getFloat();
          var summary_cu_so = CropYieldForecastFactory.summaryCultivarSoil(yield_cu, $scope.crop_yield_var.name);
          $("#yield_" + cu.id + "_max_date").html(summary_cu_so.max.date.substring(0, 10));
          $("#yield_" + cu.id + "_max_yield").html(summary_cu_so.max.value.toFixed(float) + ' ' + $scope.crop_yield_var.metric);
          $("#yield_" + cu.id + "_min_date").html(summary_cu_so.min.date.substring(0, 10));
          $("#yield_" + cu.id + "_min_yield").html(summary_cu_so.min.value.toFixed(float) + ' ' + $scope.crop_yield_var.metric);

          // The following cicle is used to draw the trend graphic
          // by every variable of the yield crop
          for (var j = 0; j < $scope.crop_vars.length; j++) {
              var vr = $scope.crop_vars[j];
              var vr_data = CropYieldForecastFactory.getByCultivarSoilMeasure(yield_cu, vr.name);

              // Draw the graphic
              var base_t = new Base('#trend_' + cu.id + '_' + vr.name, vr_data);
              base_t.setMargin(10, 50, 10, 20);
              base_t.setDateNames(setup.getMonths(), setup.getDays());
              base_t.setAxisLabelY(vr.metric);
              var trend = new Trend(base_t);
              trend.render();

              // Get the summary by measure
              var summary_vr = CropYieldForecastFactory.summaryCultivarSoilMeasure(vr_data);
              $("#yield_" + vr.name + "_" + cu.id + "_max_date").html(summary_vr.max.date.substring(0, 10));
              $("#yield_" + vr.name + "_" + cu.id + "_max_yield").html(summary_vr.max.value.toFixed(float) + ' ' + vr.metric);
              $("#yield_" + vr.name + "_" + cu.id + "_min_date").html(summary_vr.min.date.substring(0, 10));
              $("#yield_" + vr.name + "_" + cu.id + "_min_yield").html(summary_vr.min.value.toFixed(float) + ' ' + vr.metric);
          }

          // Hide the content of the tabs variation
          $('#content_' + cu.id + ' div').removeClass('active');
          $('#content_' + cu.id + ' div').removeClass('in');
          $('#content_' + cu.id + ' div:first').addClass('active');
          $('#content_' + cu.id + ' div:first').addClass('in');
      }

      $rootScope.drawFunction = function (section) {

      }

  });