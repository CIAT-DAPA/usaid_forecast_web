﻿angular.module('ForecastApp')
  .controller('ClimateHistoryCtrl', function ($rootScope, $scope, setup, tools,
                                    WeatherStationFactory,
                                    ClimateClimatologyFactory, ClimateHistoricalFactory) {
      // Get the municipality from the url
      $scope.state_name = tools.search(1);
      $scope.municipality_name = tools.search(2);
      $scope.ws_name = tools.search(3);
      //$scope.municipalities = [];
      $scope.ws = null;
      $scope.gv_months = $("#gv_months").val().split(',');
      $scope.climate_vars = setup.getClimateVars();
      // 
      $scope.cv = null;
      $scope.historical_months = null;
      $scope.climatology = null;
      $scope.climatology_filtered = null;
      $scope.cv_month_selected = null;
      //
      $scope.historical_upper = null;
      $scope.historical_lower = null;
      // Vars of the data
      // Historical data
      $scope.historical = null;
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
          WeatherStationFactory.search($scope.state_name, $scope.municipality_name, $scope.ws_name).then(
          function (data_ws) {
              $scope.ws = data_ws;

              ClimateHistoricalFactory.getByWeatherStation($scope.ws.id).then(
              function (data_h) {
                  $scope.historical = data_h;                  
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
       * (int) i: Position of the climate vars array
      */
      function draw_climatology(i) {
          $scope.cv = $scope.climate_vars[i];          
          // Get data only for the climatology vars selected
          var climatology_temp = $scope.climatology.map(function (item) {
              var data = item.filter(function (item2) { return $scope.cv.value.includes(item2.measure); });
              return data;
          });
          // Transform data for the graphic
          $scope.climatology_filtered = [];          
          for (var j = 0; j < climatology_temp.length; j++) {
              for (var k = 0; k < climatology_temp[j].length; k++)
                  $scope.climatology_filtered.push({
                      month: climatology_temp[j][k].month,
                      month_name: climatology_temp[j][k].month_name,
                      measure: climatology_temp[j][k].measure,
                      value: climatology_temp[j][k].value
                  });
          }

          // Draw the graphic          
          var base_c = new Base('#bar_climatology', $scope.climatology_filtered);
          base_c.setMargin(10, 50, 10, 10);
          base_c.setClasses($scope.cv.value.map(function (item) { return 'bar_' + item; }));
          base_c.setAxisLabelY($scope.cv.metric);
          var bar = new Bars(base_c);
          bar.render();
          var compute_c = ClimateClimatologyFactory.summary($scope.climatology_filtered);

          // Get data summary
          $scope.cv.month_start = $scope.climatology_filtered[0].month_name;
          $scope.cv.month_end = $scope.climatology_filtered[$scope.climatology_filtered.length - 1].month_name;
          $scope.cv.max = compute_c.max;
          $scope.cv.min = compute_c.min;

          var m = setup.getMonthsFull();
          var a = [];
          for (var i = 0; i < $scope.climatology_filtered.length; i++)
              a.push(m[$scope.climatology_filtered[i].month - 1]);
          
          $scope.historical_months = a.filter(function (item, pos) {
              return a.indexOf(item) == pos;
          });
          $scope.cv_month_selected = $scope.historical_months[0];
          draw_historical();
      }


      function draw_historical() {
          // Get data only for the climatology vars selected
          var historical_temp = $scope.historical.map(function (item) {
              return item.monthly_data.map(function (item2) {
                  return {
                      month: item2.month,
                      year: item.year,
                      monthly_data: item2.data.filter(function (item3) {
                          return $scope.cv.value.includes(item3.measure);
                      })
                  };
              });
          });
          // Transform data for the graphic
          var historical_filtered = [];

          for (var k = 0; k < historical_temp.length; k++) {
              for (var l = 0; l < historical_temp[k].length; l++) {
                  for (var m = 0; m < historical_temp[k][l].monthly_data.length; m++) {
                      if ($scope.cv_month_selected.id == historical_temp[k][l].month)
                          historical_filtered.push({
                              year: historical_temp[k][l].year,
                              month: historical_temp[k][l].month,
                              // Transform solar radiation
                              value: (historical_temp[k][l].monthly_data[m].measure === 'sol_rad' ? historical_temp[k][l].monthly_data[m].value / 0.041868 : historical_temp[k][l].monthly_data[m].value),
                              date: new Date(historical_temp[k][l].year, 1, 1),
                              measure: historical_temp[k][l].monthly_data[m].measure
                          });
                  }
              }
          }
          // Get the climatology for the month selected
          var climatology_line = $scope.climatology_filtered.filter(function (item) {
              return item.month == $scope.cv_month_selected.id;
          });

          var data_h = { raw: historical_filtered, splitted: climatology_line };
          //var cvm = $scope.historical_months[j];
          var base_h = new Base('#historical_content_line', data_h);
          // Build the graphic for every month
          base_h.setMargin(10, 50, 10, 10);
          base_h.setClasses($scope.cv.value);
          base_h.setAxisLabelY($scope.cv.metric);
          var line = new Line(base_h);
          line.render();

          // Add summary to the content tab          
          var summary = ClimateHistoricalFactory.summary(historical_filtered, climatology_line);
          $scope.historical_upper = summary.upper;
          $scope.historical_lower = summary.lower;
      }
  });
