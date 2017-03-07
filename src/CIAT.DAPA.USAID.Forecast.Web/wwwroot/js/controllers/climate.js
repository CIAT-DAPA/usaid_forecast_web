'use strict';

/**
 * @ngdoc function
 * @name ForecastApp.controller:ClimateCtrl
 * @description
 * # ClimateCtrl
 * Controller of the ForecastApp
 */
angular.module('ForecastApp')
  .controller('ClimateCtrl', function ($scope, config, tools, HistoricalFactory, ClimatologyFactory, HistoricalClimateFactory,
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
                                '<div class="section-content">' +
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
                                 ' mm y ' + climatology_upper[0].value.toFixed(config.float) + ' mm</span>, la predicción climática determina que ' +
                                 '<span class="text-bold">' + summary + '</span>.';
              $('#summary_' + m.year + '-' + m.month).html(summary_text);
          }

          $('#probabilities_pies section').removeClass('active');
          $('#probabilities_pies section:first').addClass('active');

          // Historical data for every climate variable
          for (var i = 0; i < $scope.climate_vars.length; i++) {
              try {
                  var cv = $scope.climate_vars[i];
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
                  base_c.setClass('bar_' + cv.value);
                  var bar = new Bars(base_c);
                  bar.render();
                  var compute_c = ClimatologyFactory.summary(climatology);

                  cv.month_start = climatology[0].month_name;
                  cv.month_end = climatology[climatology.length - 1].month_name;
                  cv.max = compute_c.max;
                  cv.min = compute_c.min;

                  // Historical

                  // Build the html code for every month of the forecast in tabs.
                  // All content of tabs is enable, so this way later can draw the graphic
                  var tabs = '<ul class="nav nav-tabs nav-justified" id="' + cv.value + '_tabs" role="tablist">';
                  var content = '<div class="tab-content" id="' + cv.value + '_content">';
                  var tab_enable = '';
                  for (var j = 0; j < cv.historical_months.length; j++) {
                      var cvm = cv.historical_months[j];
                      tabs += '<li role="presentation"' + (j == 0 ? ' class="active"' : '') + '>' +
                                '<a href="#' + cv.value + '_' + cvm + '_content" id="' + cv.value + '_' + cvm + '_tab" role="tab" data-toggle="tab" aria-controls="' + cv.value + '_' + cvm + '_content"> ' + cvm + '</a>' +
                             '</li>';
                      content += '<div class="tab-pane fade active in ' + cv.value + '" role="tabpanel" id="' + cv.value + '_' + cvm + '_content" aria-labelledby="' + cv.value + '_' + cvm + '_tab">' +
                                    '<p class="text-justify" id="' + cv.value + '_' + cvm + '_summary">' +
                                    '</p>' +
                                    '<div id="' + cv.value + '_line_historical_' + cvm + '">' +
                                    '</div>' +
                                '</div>';
                      if (j == 0)
                          tab_enable = cv.value + '_' + cvm + '_content';
                  }
                  tabs += '</ul>';
                  content += '</div>';
                  $('#climatic_history_content_' + cv.value).html(tabs + content);
                  // Add the event to show the tabs on click
                  $('#' + cv.value + '_tabs a').click(function (e) {
                      e.preventDefault();
                      $(this).tab('show');
                  });
                  // Add the line grapich for every month
                  for (var j = 0; j < cv.historical_months.length; j++) {
                      // Get data from month
                      var cvm = cv.historical_months[j];
                      var historical = HistoricalClimateFactory.getData($scope.data_h, $scope.ws_entity.id, h_month_start, cv.value);
                      var data_h = { raw: historical, splitted: climatology[j].value };
                      var base_h = new Base('#' + cv.value + '_line_historical_' + cvm, data_h);
                      // Build the graphic for every month
                      base_h.setMargin(10, 30, 10, 10);
                      base_h.setClass(cv.value);
                      var line = new Line(base_h);
                      line.render();
                      h_month_start += 1;
                      // Add summary to the content tab
                      var summary_data = HistoricalClimateFactory.summary(historical, climatology[j].value);
                      var summary = 'Históricamente en el mes <span class="text-bold">' + cvm + '</span> en el ' +
                                    'municipio <span class="text-bold">' + $scope.municipality_name + '</span> presenta el siguiente comportamiento:' +
                                    '<ul>' +
                                        '<li>Han habido <span class="text-bold">' + summary_data.upper + '</span> años por encima de lo normal</li>' +
                                        '<li>Han habido <span class="text-bold">' + summary_data.lower + '</span> años por debajo de lo normal</li>' +
                                    '</ul>';
                      $('#' + cv.value + '_' + cvm + '_summary').html(summary);
                  }
                  // Disable the content of tabs (hide the content)
                  $('.' + cv.value).removeClass('active');
                  $('.' + cv.value).removeClass('in');
                  // Enable the content of the first tab
                  $('#' + tab_enable).addClass('active');
                  $('#' + tab_enable).addClass('in');
              }
              catch (err) {
                  console.log(err);
              }
          }
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
          var data = AssistFactory.getByIdAlt(id,alt);
          tools.show_assist(data.title, data.text, data.url);
          return false;
      }
  });