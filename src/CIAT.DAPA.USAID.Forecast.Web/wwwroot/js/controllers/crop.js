'use strict';

/**
 * @ngdoc function
 * @name ForecastApp.controller:CropCtrl
 * @description
 * # CropCtrl
 * Controller of the ForecastApp
 */
angular.module('ForecastApp')
  .controller('CropCtrl', function ($scope, config, tools, HistoricalFactory, ClimatologyFactory, HistoricalClimateFactory, ForecastFactory, ClimateFactory, GeographicFactory, MunicipalityFactory, WeatherStationFactory) {
      $scope.crop_name = tools.search('cultivo');
      // Get the municipality from the url
      $scope.municipality_name = tools.search('municipio');
      $scope.municipalities = [];
      $scope.ws_entity = null;
      $scope.gv_months = $("#gv_months").val().split(',');
      $scope.gv_municipalities = $("#gv_municipalities").val().split(',');
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
          $scope.municipalities = MunicipalityFactory.listByIds(data_m, $scope.gv_municipalities);
          // Search the weather station
          $scope.ws_entity = WeatherStationFactory.getByMunicipality(data_m, $scope.municipality_name);
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

      /*
       * Method that draw in screen the information getted from the web api
      */
      function draw() {
          // Forecast
          var months = ClimateFactory.getProbabilities($scope.data_f, $scope.ws_entity.id, 'prec');
          var ctrs = '';
          var period = '';
          // This cicle add the html code for the graphic pie.
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
          // This cicle add the graphic pie and render the information of the forecast
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
                      var line = new Line(base_h);
                      line.render();
                      h_month_start += 1;
                      // Add summary to the content tab
                      var summary_data = HistoricalClimateFactory.summary(historical, climatology[j].value);
                      var summary = 'Históricamente en el mes <span class="text-bold">' + cvm + '</span> en el ' +
                                    'municipio <span class="text-bold">' + $scope.municipality_name + '</span> presenta el siguiente comportamiento:' +
                                    '<ul>' +
                                        '<li>Han habido <span class="text-bold">' + summary_data.upper + '</span> anos por encima de lo normal</li>' +
                                        '<li>Han habido <span class="text-bold">' + summary_data.lower + '</span> anos por debajo de lo normal</li>' +
                                        '<li>Han habido <span class="text-bold">' + summary_data.equals + '</span> anos similar a lo normal</li>' +
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

  });