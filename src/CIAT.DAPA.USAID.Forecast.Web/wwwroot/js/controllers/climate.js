'use strict';

/**
 * @ngdoc function
 * @name ForecastApp.controller:ClimateCtrl
 * @description
 * # ClimateCtrl
 * Controller of the ForecastApp
 */
angular.module('ForecastApp')
  .controller('ClimateCtrl', function ($scope, config, tools, HistoricalFactory, ClimatologyFactory, HistoricalClimateFactory, ForecastFactory, ClimateFactory, GeographicFactory, MunicipalityFactory, WeatherStationFactory) {
      // Get the municipality from the url
      $scope.municipality_name = tools.search('municipio');
      $scope.municipalities = [];
      $scope.ws_entity = null;
      $scope.gv_months = $("#gv_months").val().split(',');
      $scope.climate_vars = config.climate_vars;
      $scope.period_start = null;
      $scope.period_end = null;
      // Vars of the data
      // Data municipalities
      $scope.data_m = null;
      // Historical data
      $scope.data_h = null;
      // Forecast data
      $scope.data_f = null;

      // Load data from web web api
      // Get all geographic data able with information
      GeographicFactory.get().success(function (data_m) {
          $scope.data_m = data_m;
          // List all municipalities
          $scope.municipalities = MunicipalityFactory.listAll(data_m);
          // Search the weather station
          $scope.ws_entity = WeatherStationFactory.getByMunicipality(data_m, $scope.municipality_name);
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

      /*
       * Method that draw in screen the information getted from the web api
      */
      function draw() {
          // Forecast
          var months = ClimateFactory.getProbabilities($scope.data_f, $scope.ws_entity.id, 'prec');
          var ctrs = '';
          var period = '';
          for (var i = 0; i < months.length; i++) {
              var m = months[i];
              if (i == 0)
                  period = m.month_name + ', ' + m.year + ' a ';
              else if (i == (months.length - 1))
                  period = period + m.month_name + ', ' + m.year;
              ctrs = ctrs + '<article class="col-lg-2 article_content">' +
                                '<div class="section-content">' +
                                    '<h3 class="text-center">' + m.year + '-' + m.month_name + '</h3>' +
                                    '<h4 class="text-center">Precipitación</h4>' +
                                    '<div id="pie' + m.year + '-' + m.month + '"></div>' +
                                    '<p class="text-justify article_content">' +
                                        'Para el mes de <span class="text-bold">' + m.month_name + '</span> ' +
                                        'se predice que <span class="text-bold">' + m.summary + '</span> ' +
                                    '</p>' +
                                '</div>' +
                            '</article>';
          }
          $('#climate-period').html(period);
          $("#probabilities_pies").html(ctrs);          
          for (var i = 0; i < months.length; i++) {
              var m = months[i];
              if (i == 0)
                  $scope.period_start = m.month_name + ', ' + m.year;
              if (i == (months.length - 1))
                  $scope.period_end = m.month_name + ', ' + m.year;
              var id = '#pie' + m.year + '-' + m.month;
              var climatology = ClimatologyFactory.getMonthlyData($scope.data_h, $scope.ws_entity.id, [m.month.toString()], 'prec');              
              var data = { percentages: m.probabilities, center: climatology[0].value };
              var base = new Base(id, data);
              var pie = new Pie(base);
              pie.render();
          }

          // Historical data for every climate variable
          for (var i = 0; i < $scope.climate_vars.length; i++) {
              try {
                  var cv = $scope.climate_vars[i];
                  // Climatology
                  var climatology = ClimatologyFactory.getMonthlyData($scope.data_h, $scope.ws_entity.id, $scope.gv_months, cv.value);                  
                  var base_c = new Base('#' + cv.value + '_bar_climatology', climatology);
                  base_c.setMargin(10, 30, 10, 10);
                  var bar = new Bars(base_c);
                  bar.render();
                  var compute_c = ClimatologyFactory.summary(climatology);
                  var summary_c = 'La climatología para el período de <span class="text-bold">' + $scope.period_start + ' a ' + $scope.period_end + '</span>, ' +
                            'en el municipio <span class="text-bold">' + $scope.municipality_name + '</span> nos indica que: ' +
                            '<ul>' +
                                '<li>El mes <span class="text-bold">' + compute_c.max.month_name + '</span> ha tenido mayores valores de <span class="text-bold">' + cv.name + ' (' + compute_c.max.value.toFixed(1) + ' ' + cv.metric + ')</span>' + '</li>' +
                                '<li>El mes <span class="text-bold">' + compute_c.min.month_name + '</span> ha tenido menores valores de <span class="text-bold">' + cv.name + ' (' + compute_c.min.value.toFixed(1) + ' ' + cv.metric + ')</span>' + '</li>' +
                                '<li>Existe una variación de <span class="text-bold">' + compute_c.distance.toFixed(1) + ' ' + cv.metric + '</span> en el semestre</li>' +
                            '</ul>';
                  $('#' + cv.value + '_content_climatology').html(summary_c);

                  // Historical
                  var historical = HistoricalClimateFactory.getData($scope.data_h, $scope.ws_entity.id, 1, cv.value);
                  var base_h = new Base('#' + cv.value + '_line_historical', historical);
                  base_h.setMargin(10, 30, 10, 10);
                  var line = new Line(base_h);
                  line.render();
              }
              catch (err) {
                  console.log(err);
              }
          }
      }

  });