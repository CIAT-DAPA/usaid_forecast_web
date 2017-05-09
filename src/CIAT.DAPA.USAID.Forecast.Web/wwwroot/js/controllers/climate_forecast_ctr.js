angular.module('ForecastApp')
  .controller('ClimateForecastCtrl', function ($rootScope, $scope, setup, tools,
                                    WeatherStationFactory,
                                    ClimateClimatologyFactory,
                                    ClimateForecastFactory) {
      // Get the municipality from the url
      $scope.municipality_name = tools.search('municipio');
      $scope.period_start = null;
      $scope.period_end = null;
      $scope.gv_months = $("#gv_months").val().split(',');
      // Vars of the data
      // Weather Station
      $scope.ws = null;
      // Climatology data
      $scope.climatology_lower = null;
      $scope.climatology_upper = null;
      // Forecast data
      $scope.forecast = null;
      // Months
      $scope.months = null;

      // Load data from web web api
      WeatherStationFactory.getByMunicipality($scope.municipality_name)
        .then(function (data_ws) {
            $scope.ws = data_ws;
            ClimateForecastFactory.getProbabilities($scope.ws.id, 'prec').
                then(function (data_fs) {
                    $scope.forecast = data_fs;
                    $scope.months = $scope.forecast.map(function (item) { return item.month.toString(); });
                    ClimateClimatologyFactory.getMonthlyData($scope.ws.id, $scope.months, setup.getClimatologyVarsForecast().lower).then(
                        function (data_l) {
                            $scope.climatology_lower = data_l;
                            ClimateClimatologyFactory.getMonthlyData($scope.ws.id, $scope.months, setup.getClimatologyVarsForecast().upper).then(
                            function (data_u) {
                                $scope.climatology_upper = data_u;

                                // Draw graphic
                                draw_forecast();
                            },
                            function (err) {
                                console.log(err);
                            });
                        },
                        function (err) {
                            console.log(err);
                        });
                },
                function (err) {
                    console.log(err);
                });
        },
        function (err) {
            console.log(err);
        });
      /*
      // Get all geographic data able with information
      GeographicFactory.get().success(function (data_m) {
          $scope.data_m = data_m;
          // List all municipalities
          //$scope.municipalities = MunicipalityFactory.listAll(data_m);
          // Search the weather station
          $scope.ws_entity = WeatherStationFactory.getByMunicipality(data_m, );
          // Load the historical information
          HistoricalFactory.get($scope.ws_entity.id).success(function (data_h) {
              $scope.data_h = data_h;
              // Load the Forecast information
              ForecastFactory.get().success(function (data_f) {
                  $scope.data_f = data_f;

                  // By default its draw the forecast climate
                  draw_forecast();
              }).error(function (error) {
                  console.log(error);
              });
          }).error(function (error) {
              console.log(error);
          });
      }).error(function (error) {
          console.log(error);
      });
      */

      $rootScope.drawFunction = function (section) {
          draw_forecast();
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
          for (var i = 0; i < $scope.forecast.length; i++) {
              var m = $scope.forecast[i];
              var cl_lower = $scope.climatology_lower[i].value.toFixed(setup.getFloat());
              var cl_upper = $scope.climatology_upper[i].value.toFixed(setup.getFloat());
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
              var summary = ClimateForecastFactory.summary(m);
              var summary_text = 'Para el mes <span class="text-bold">' + m.month_name + '</span> ' +
                                 'en el municipio <span class="text-bold">' + $scope.municipality_name + '</span> ' +
                                 'lo normal es que haya una precipitación entre <span class="text-bold">' + cl_lower +
                                 ' mm y ' + cl_upper + ' mm</span>, la predicción climática sugiere que ' +
                                 '<span class="text-bold">' + summary + '</span>.';
              $('#summary_' + m.year + '-' + m.month).html(summary_text);
          }

          $('#probabilities_pies section').removeClass('active');
          $('#probabilities_pies section:first').addClass('active');
      }
  });
