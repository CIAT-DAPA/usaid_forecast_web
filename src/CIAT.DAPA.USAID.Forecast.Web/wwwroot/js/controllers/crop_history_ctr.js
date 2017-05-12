'use strict';

/**
 * @ngdoc function
 * @name ForecastApp.CropHistoryCtrl
 * @description
 * # CropHistoryCtrl
 * Crop History of the ForecastApp
 */
angular.module('ForecastApp')
  .controller('CropHistoryCtrl', function ($scope, config, tools, HistoricalFactory, ForecastFactory, GeographicFactory,
                                    MunicipalityFactory, WeatherStationFactory, AgronomicFactory, CultivarsFactory,
                                    SoilFactory, YieldForecastFactory, CropVarsFactory, GuildFactory, HistoricalYieldFactory,
                                    AssistFactory, $rootScope) {
      $scope.crop_name = tools.search('cultivo');
      // Get vars to show by crop
      $scope.crop_vars = CropVarsFactory.getVarsByCrop($scope.crop_name);
      $scope.crop_yield_var = CropVarsFactory.getDefaultVarByCrop($scope.crop_name);
      // Get the guild
      $scope.guild = GuildFactory.getByCrop($scope.crop_name);
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
       * Method that filter the data to show the information in the screen getted from the web api
      */
      function fixed_data_forecast(i) {

          var cu = $scope.cultivars[i];

          // Enable tabs for the variation graphic
          // Add the event to show the tabs on click
          $('#tabs_' + cu.id + ' a').click(function (e) {
              e.preventDefault();
              $(this).tab('show');
          });

          draw_forecast(cu, $scope.cultivars[i].soils[0]);

          // Hide the content of the tabs variation
          $('.disable_tab').removeClass('active');
          $('.disable_tab').removeClass('in');

      }

      /*
       * Method that draw the graphics by a cultivar and soil of the forecast
       * (object) cu: Cultivar entity
       * (object) so: Soil entity
      */
      function draw_forecast(cu, so) {
          // Set default color for all buttons of the soil by the cultivar
          $('#navbar_cultivar_' + cu.id + ' button').removeClass('btn-primary');
          $('#soil_' + cu.id + '_' + so.id).addClass('btn-primary');

          // Get data
          var yield_cu = YieldForecastFactory.getByCultivarSoil($scope.yield_ws.yield, cu.id, so.id);

          // Draw the graphic
          var base_c = new Base('#calendar_' + cu.id, yield_cu);
          base_c.setMargin(10, 30, 10, 10);
          var calendar = new Calendar(base_c, config.month_names, config.days_names, $scope.crop_yield_var.name, '#back_' + cu.id, '#forward_' + cu.id, '#current_month_' + cu.id, $scope.yield_ranges, 'c_alias_' + cu.id);
          calendar.render();

          // Get the summary 
          var summary_cu_so = YieldForecastFactory.summaryCultivarSoil(yield_cu, $scope.crop_yield_var.name);
          $("#yield_" + cu.id + "_max_date").html(summary_cu_so.max.date.substring(0, 10));
          $("#yield_" + cu.id + "_max_yield").html(summary_cu_so.max.value.toFixed(config.float) + ' ' + $scope.crop_yield_var.metric);
          $("#yield_" + cu.id + "_min_date").html(summary_cu_so.min.date.substring(0, 10));
          $("#yield_" + cu.id + "_min_yield").html(summary_cu_so.min.value.toFixed(config.float) + ' ' + $scope.crop_yield_var.metric);

          // The following cicle is used to draw the trend graphic
          // by every variable of the yield crop
          for (var j = 0; j < $scope.crop_vars.length; j++) {
              var vr = $scope.crop_vars[j];
              var vr_data = YieldForecastFactory.getByCultivarSoilMeasure(yield_cu, vr.name);

              // Draw the graphic
              var base_t = new Base('#trend_' + cu.id + '_' + vr.name, vr_data);
              base_t.setMargin(10, 50, 10, 20);
              base_t.setDateNames(config.month_names, config.days_names);
              base_t.setAxisLabelY(vr.metric);
              var trend = new Trend(base_t);
              trend.render();

              // Get the summary by measure
              var summary_vr = YieldForecastFactory.summaryCultivarSoilMeasure(vr_data);
              $("#yield_" + vr.name + "_" + cu.id + "_max_date").html(summary_vr.max.date.substring(0, 10));
              $("#yield_" + vr.name + "_" + cu.id + "_max_yield").html(summary_vr.max.value.toFixed(config.float) + ' ' + vr.metric);
              $("#yield_" + vr.name + "_" + cu.id + "_min_date").html(summary_vr.min.date.substring(0, 10));
              $("#yield_" + vr.name + "_" + cu.id + "_min_yield").html(summary_vr.min.value.toFixed(config.float) + ' ' + vr.metric);
          }
      }

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

      /*
       * Method to search assist data and show in the modal window
       * (string) id: Helper name
      */
      $scope.assist = function (id) {
          var data = AssistFactory.getById(id);
          tools.show_assist(data.title, data.text, data.url);
          return false;
      }

      $rootScope.drawFunction = function () {
          console.log("test");
      }

  });