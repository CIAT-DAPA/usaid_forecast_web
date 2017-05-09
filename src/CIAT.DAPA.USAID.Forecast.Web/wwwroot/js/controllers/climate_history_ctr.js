angular.module('ForecastApp')
  .controller('ClimateHistoryCtrl', function ($rootScope, $scope, setup, tools,
                                    WeatherStationFactory,
                                    ClimateClimatologyFactory, ClimateHistoricalFactory) {
      // Get the municipality from the url
      $scope.municipality_name = tools.search('municipio');
      //$scope.municipalities = [];
      $scope.ws = null;
      $scope.gv_months = $("#gv_months").val().split(',');
      $scope.climate_vars = setup.getClimateVars();
      // 
      $scope.cv = null;
      $scope.historical_months = null;
      $scope.climatology = null;
      $scope.cv_month_selected = null;
      // Vars of the data
      // Historical data
      $scope.data_h = null;
      $scope.loaded = false;

      load_data();

      $scope.drawHistorical = draw_historical;

      $rootScope.drawFunction = function (section) {
          if (!$scope.loaded)
              load_data();

          if (section === 'precipitation')
              draw_climatology(0);
          else if (section === 'temperature')
              draw_climatology(1);
          else if (section === 'solar_radiation')
              draw_climatology(2);
      }

      function load_data() {
          // Load data from web web api
          // Get data of the weather station
          WeatherStationFactory.getByMunicipality($scope.municipality_name).then(
          function (data_ws) {
              $scope.ws = data_ws;

              ClimateHistoricalFactory.getByWeatherStation($scope.ws.id).then(
              function (data_h) {
                  $scope.data_h = data_h;
                  ClimateClimatologyFactory.getMonthly($scope.ws.id, $scope.gv_months).then(
                  function (data_c) {
                      $scope.climatology = data_c;
                      $scope.loaded = true;
                  },
                  function (err) { console.log(err); });
              },
              function (err) { console.log(err); });
          },
          function (err) { console.log(err); });
      }

      /*
       * Method that draw in screen the information getted from the web api about
      */
      function draw_climatology(i) {
          $scope.cv = $scope.climate_vars[i];

          // Get data only for the climatology vars selected
          var climatology_temp = $scope.climatology.map(function (item) {
              var data = item.filter(function (item2) { return $scope.cv.value.includes(item2.measure); });
              return data;
          });

          // Transform data for the graphic
          var data_climatology = [];
          for (var j = 0; j < climatology_temp.length; j++) {
              for (var k = 0; k < climatology_temp[j].length; k++)
                  data_climatology.push({
                      month: climatology_temp[j][k].month,
                      month_name: climatology_temp[j][k].month_name,
                      measure: climatology_temp[j][k].measure,
                      value: climatology_temp[j][k].value
                  });
          }

          // Draw the graphic
          var base_c = new Base('#bar_climatology', data_climatology);
          base_c.setMargin(10, 30, 10, 10);
          base_c.setClasses($scope.cv.value.map(function (item) { return 'bar_' + item; }));
          base_c.setAxisLabelY($scope.cv.metric);
          var bar = new Bars(base_c);
          bar.render();
          var compute_c = ClimateClimatologyFactory.summary(climatology_temp);

          $scope.cv.month_start = $scope.climatology[0].month_name;
          $scope.cv.month_end = $scope.climatology[$scope.climatology.length - 1].month_name;
          $scope.cv.max = compute_c.max;
          $scope.cv.min = compute_c.min;

          /*$scope.historical_months = setup.getMonths().slice(h_month_start - 1, h_month_end);
          $scope.cv_month_selected = 0;
          draw_historical();*/
      }

      function draw_historical() {
          // Set the months for historical data          
          var h_month_start = parseInt($scope.gv_months[0]);
          var h_month_end = parseInt($scope.gv_months[$scope.gv_months.length - 1]);

          var j = $scope.cv_month_selected;
          var h_month_start = parseInt($scope.gv_months[0]);
          var h_month_end = parseInt($scope.gv_months[$scope.gv_months.length - 1]);

          var cvm = $scope.historical_months[j];
          var historical = HistoricalClimateFactory.getData($scope.data_h, $scope.ws_entity.id, h_month_start, $scope.cv.value[0]);
          console.log($scope.climatology[j]);
          var data_h = { raw: historical, splitted: $scope.climatology[j].value };
          var base_h = new Base('#historical_content_line', data_h);
          // Build the graphic for every month
          base_h.setMargin(10, 30, 10, 10);
          base_h.setClass($scope.cv.value);
          base_h.setAxisLabelY($scope.cv.metric);
          var line = new Line(base_h);
          line.render();
          h_month_start += 1;
          // Add summary to the content tab
          /*var summary_data = HistoricalClimateFactory.summary(historical, climatology[j].value);
          var summary = 'Históricamente en el mes <span class="text-bold">' + cvm + '</span> en el ' +
                        'municipio <span class="text-bold">' + $scope.municipality_name + '</span> presenta el siguiente comportamiento:' +
                        '<ul>' +
                            '<li>Se han presentado <span class="text-bold">' + summary_data.upper + '</span> años por encima de lo normal</li>' +
                            '<li>Se han presentado <span class="text-bold">' + summary_data.lower + '</span> años por debajo de lo normal</li>' +
                        '</ul>';
          $('#' + cv.container + '_' + cvm + '_summary').html(summary);*/
      }
  });
