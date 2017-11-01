angular.module('ForecastApp')
  .controller('ClimateForecastCtrl', function ($rootScope, $scope, setup, tools,
                                    WeatherStationFactory,
                                    ClimateClimatologyFactory,
                                    ClimateForecastFactory, ClimateScenarioFactory) {
      // Get the municipality from the url
      $scope.municipality_name = tools.search('municipio');
      // Vars view
      $scope.period_start = null;
      $scope.period_end = null;
      // Weather Station
      $scope.ws = null;
      // Climatology data
      $scope.climatology_lower = null;
      $scope.climatology_upper = null;
      // Forecast data
      $scope.forecast = null;
      // Forecast scenario
      $scope.scenario = null;
      // Months
      $scope.months = null;
      $scope.loaded = false;

      load_data();

      /*
       * Method that render the data in the screen
       * (string) section: Section name to draw
      */
      $rootScope.drawFunction = function (section) {
          if (!$scope.loaded)
              load_data();
          else
              draw_forecast();
      }

      function load_data() {
          // Load data from web web api
          // Get data of the weather station
          WeatherStationFactory.getByMunicipality($scope.municipality_name).then(
          function (data_ws) {
              $scope.ws = data_ws;
              // Get climate forecast data of the precipitation
              ClimateForecastFactory.getProbabilities($scope.ws.id, 'prec', true).then(
              function (data_fs) {
                  $scope.forecast = data_fs;
                  // Get the months of the forecast
                  $scope.months = $scope.forecast.map(function (item) {
                      return item.month.toString().length == 1 ? '0' + item.month.toString() : item.month.toString();
                  });

                  // Get scenarios
                  ClimateScenarioFactory.getScenarios($scope.ws.id).then(
                  function (data_fs) {
                      $scope.scenario = data_fs;
                      // Get limit lower of the climatology for the months of the forecast                       
                      ClimateClimatologyFactory.getMonthlyData($scope.ws.id, $scope.months, setup.getClimatologyVarsForecast().lower).then(
                      function (data_l) {                          
                          $scope.climatology_lower = data_l;
                          // Get limit upper of the climatology for the months of the forecast
                          ClimateClimatologyFactory.getMonthlyData($scope.ws.id, $scope.months, setup.getClimatologyVarsForecast().upper).then(
                          function (data_u) {
                              $scope.climatology_upper = data_u;
                              // Draw graphic
                              draw_forecast();
                              // Close loading 
                              tools.updateBackground();
                              window.loading_screen.finish();
                              // Show tutorial
                              $rootScope.showTutorial();
                          },
                          function (err) { console.log(err); });
                      },
                      function (err) { console.log(err); });
                  },
                  function (err) { console.log(err); });
              },
              function (err) { console.log(err); });
          },
          function (err) { console.log(err); });
      }

      /*
       * Method that draw in screen the information getted from the web api about forecast
      */
      function draw_forecast() {
          // Forecast          
          var ctrs = '<div id="climate_carousel" class="carousel slide" data-ride="carousel">' +
                        '<ol class="carousel-indicators">' +
                            '<li data-target="#climate_carousel" data-slide-to="0" class="active"></li>' +
                            '<li data-target="#climate_carousel" data-slide-to="1"></li>' +
                            '<li data-target="#climate_carousel" data-slide-to="2"></li>' +
                            '<li data-target="#climate_carousel" data-slide-to="3"></li>' +
                            '<li data-target="#climate_carousel" data-slide-to="4"></li>' +
                            '<li data-target="#climate_carousel" data-slide-to="5"></li>' +
                        '</ol>' +
                        '<div class="carousel-inner" role="listbox">';
          var period = '';
          // This cicle add the html code for the graphic pie.
          for (var i = 0; i < $scope.forecast.length; i++) {
              var m = $scope.forecast[i];
              if (i == 0)
                  period = m.month_name + ', ' + m.year + ' a ';
              else if (i == ($scope.forecast.length - 1))
                  period = period + m.month_name + ', ' + m.year;
              //if (i == 0 || i == 2 || i == 4)
              ctrs = ctrs + '<section class="item active">';
              ctrs = ctrs + '<article class="col-lg-5 article_content col-sm-offset-1">' +
                                '<div class="section-content">' +
                                    '<h3 class="text-center">' + m.month_name + '-' + m.year + '</h3>' +
                                    '<h4 class="text-center">Probabilidad de precipitación (%)</h4>' +
                                    '<div id="pie' + m.year + '-' + m.month + '"></div>' +
                                    '<p class="text-justify article_content" id="summary_' + m.year + '-' + m.month + '">' +
                                    '</p>' +
                                '</div>' +
                            '</article>';
              ctrs = ctrs + '<article class="col-lg-5 article_content">' +
                                '<div class="section-content">' +
                                    '<h3 class="text-center">Escenarios de la predicción</h3>' +
                                    '<div id="table' + m.year + '-' + m.month + '"></div>' +
                                '</div>' +
                            '</article>';
              //if (i == 1 || i == 3 || i == 5)
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
          for (var i = 0; i < $scope.forecast.length; i++) {
              var m = $scope.forecast[i];
              var cl_lower = $scope.climatology_lower.filter(function (item) { return item.month == m.month; })[0].value.toFixed(setup.getFloat());
              var cl_upper = $scope.climatology_upper.filter(function (item) { return item.month == m.month; })[0].value.toFixed(setup.getFloat());
              if (i == 0)
                  $scope.period_start = m.month_name + ', ' + m.year;
              if (i == ($scope.months.length - 1))
                  $scope.period_end = m.month_name + ', ' + m.year;
              var id = '#pie' + m.year + '-' + m.month;
              var data = { percentages: m.probabilities, center: '[' + cl_lower + ' - ' + cl_upper + ']' };
              var base = new Base(id, data);
              var pie = new Pie(base);
              pie.render();

              // Add summary
              var summary = ClimateForecastFactory.summary(m.probabilities);
              var summary_text = 'Para el mes de <span class="text-bold">' + m.month_name + '</span> ' +
                                 'en el municipio <span class="text-bold">' + $scope.municipality_name + '</span> ' +
                                 'lo normal es que haya una precipitación entre <span class="text-bold">' + cl_lower +
                                 ' mm y ' + cl_upper + ' mm</span>, la predicción climática sugiere que lo más probable es que ' +
                                 '<span class="text-bold">' + summary + '</span>.';
              $('#summary_' + m.year + '-' + m.month).html(summary_text);

              // Scenarios
              var id_scenario = '#table' + m.year + '-' + m.month;
              var content_scenario = '<table class="table">' +
                                        '<tr><th>Variable</th><th>Minímo</th><th>Promedio</th><th>Máximo</th></tr>';
              var cl_vars = setup.getClimateVarsScenario();
              var scenarios_vars = ['min', 'avg', 'max'];
              var scenario_year = $scope.scenario.filter(function (item) { return item.year == m.year; });

              for (var k = 0; k < cl_vars.length; k++) {
                  content_scenario = content_scenario + '<tr><td>' + cl_vars[k].name + '</td>';
                  for (var l = 0; l < scenarios_vars.length; l++) {
                      var s = scenario_year.filter(function (item) { return item.name === scenarios_vars[l]; })[0];
                      var s_m = s.monthly_data.filter(function (item) { return item.month == m.month; })[0];
                      var s_m_v = s_m.data.filter(function (item) { return item.measure === cl_vars[k].value; })[0];
                      content_scenario = content_scenario + '<td>' + s_m_v.value.toFixed(setup.getFloat()) + ' ' + cl_vars[k].metric + '</td>';
                  }
                  content_scenario = content_scenario + '</tr>';
              }
              content_scenario = content_scenario + '</table>';
              $(id_scenario).html(content_scenario);
          }

          $('#probabilities_pies section').removeClass('active');
          $('#probabilities_pies section:first').addClass('active');
      }
  });
