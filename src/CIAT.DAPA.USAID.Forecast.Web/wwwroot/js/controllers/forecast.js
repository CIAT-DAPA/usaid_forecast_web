'use strict';

/**
 * @ngdoc function
 * @name ForecastApp.controller:ClimateCtrl
 * @description
 * # ForecastCtrl
 * Controller of the ForecastApp
 */
angular.module('ForecastApp')
  .controller('ForecastCtrl', function ($scope, config, tools, HistoricalFactory, ClimatologyFactory, HistoricalClimateFactory,
                                    ForecastFactory, ClimateFactory, GeographicFactory, MunicipalityFactory,
                                    WeatherStationFactory, AssistFactory) {
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
          var ctrs = '<div id="climate_carousel" class="carousel slide" data-ride="carousel">' +
                        '<ol class="carousel-indicators">' +
                            '<li data-target="#climate_carousel" data-slide-to="0" class="active"></li>' +
                            '<li data-target="#climate_carousel" data-slide-to="1"></li>' +
                            '<li data-target="#climate_carousel" data-slide-to="2"></li>' +
                        '</ol>' +
                        '<div class="carousel-inner" role="listbox">';
          var period = '';
          // This cicle add the html code for the graphic pie.
          for (var i = 0; i < months.length; i++) {
              var m = months[i];
              if (i == 0)
                  period = m.month_name + ', ' + m.year + ' a ';
              else if (i == (months.length - 1))
                  period = period + m.month_name + ', ' + m.year;
              if (i == 0 || i == 2 || i == 4)
                  ctrs = ctrs + '<section class="item active">';
              ctrs = ctrs + '<article class="col-lg-4 article_content ' + ((i == 0 || i == 2 || i == 4) ? 'col-lg-offset-2' : '') + '">' +
                                '<div class="borderBox">' +
                                    '<h3 class="text-center">' + m.month_name + '-' + m.year + '</h3>' +
                                    '<h4 class="text-center">Precipitación</h4>' +
                                    '<div id="pie' + m.year + '-' + m.month + '"></div>' +
                                    '<p class="text-justify article_content" id="summary_' + m.year + '-' + m.month + '">' +
                                    '</p>' +
                                '</div>' +
                            '</article>';
              if (i == 1 || i == 3 || i == 5)
                  ctrs = ctrs + '</section>';
          }
          ctrs = ctrs + '<a class="left carousel-control" href="#climate_carousel" role="button" data-slide="prev"> ' +
                            '<span class="glyphicon glyphicon-chevron-left" aria-hidden="true"></span>' +
                            '<span class="sr-only">Previous</span>' +
                        '</a>' +
                        '<a class="right carousel-control" href="#climate_carousel" role="button" data-slide="next">' +
                            '<span class="glyphicon glyphicon-chevron-right" aria-hidden="true"></span>' +
                            '<span class="sr-only">Next</span>' +
                        '</a>' +
                 '</div>';
          $('#climate-period').html(period);
          $("#probabilities_pies").html(ctrs);
          // This cicle add the graphic pie and render the information of the forecast
          for (var i = 0; i < months.length; i++) {
              var m = months[i];
              if (i == 0)
                  $scope.period_start = m.month_name + ', ' + m.year;
              if (i == (months.length - 1))
                  $scope.period_end = m.month_name + ', ' + m.year;
              var id = '#pie' + m.year + '-' + m.month;
              var climatology_lower = ClimatologyFactory.getMonthlyData($scope.data_h, $scope.ws_entity.id, [m.month.toString()], config.climatology_forecast.lower);
              var climatology_upper = ClimatologyFactory.getMonthlyData($scope.data_h, $scope.ws_entity.id, [m.month.toString()], config.climatology_forecast.upper);
              var data = { percentages: m.probabilities, center: '[' + climatology_lower[0].value.toFixed(config.float) + ' - ' + climatology_upper[0].value.toFixed(config.float) + ']' };
              var base = new Base(id, data);
              var pie = new Pie(base);
              pie.render();

              // Add summary
              var summary = ClimateFactory.summary(m.raw);
              var summary_text = 'Para el mes <span class="text-bold">' + m.month_name + '</span> ' +
                                 'en el municipio <span class="text-bold">' + $scope.municipality_name + '</span> ' +
                                 'lo normal es que haya una precipitación entre <span class="text-bold">' + climatology_lower[0].value.toFixed(config.float) +
                                 ' mm y ' + climatology_upper[0].value.toFixed(config.float) + ' mm</span>, la predicción climática sugiere que ' +
                                 '<span class="text-bold">' + summary + '</span>.';
              $('#summary_' + m.year + '-' + m.month).html(summary_text);
          }

          $('#probabilities_pies section').removeClass('active');
          $('#probabilities_pies section:first').addClass('active');
      }

      /*
       * Method to search assist data and show in the modal window
       * (string) id: Helper name
      */
      $scope.assist = function (id) {
          var data = AssistFactory.getById(id);
          tools.show_assist(data.title, data.text, data.url);
          return false;
      }

      /*
       * Method to search assist data and show in the modal window
       * (string) id: Helper name
      */
      $scope.assist_alt = function (id, alt) {
          var data = AssistFactory.getByIdAlt(id, alt);
          tools.show_assist(data.title, data.text, data.url);
          return false;
      }
  });