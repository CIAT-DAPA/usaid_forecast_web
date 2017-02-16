'use strict';

/**
 * @ngdoc function
 * @name ForecastApp.controller:CropCtrl
 * @description
 * # CropCtrl
 * Controller of the ForecastApp
 */
angular.module('ForecastApp')
  .controller('ExpertCtrl', function ($scope, config, tools, HistoricalFactory, ForecastFactory, GeographicFactory, MunicipalityFactory, WeatherStationFactory, AgronomicFactory, CultivarsFactory, SoilFactory, YieldForecastFactory, CropVarsFactory) {
      $scope.url = {};
      $scope.url.geographic = GeographicFactory.getUrl();
      $scope.url.agronomic = AgronomicFactory.getUrl();
      $scope.url.climatology = HistoricalFactory.getUrl('');
      $scope.url.historical_climate = HistoricalFactory.getUrl('');

      $scope.data_title = '';

      $scope.climatology = {};
      $scope.historical_climate = {};

      // Vars of the data
      // Data municipalities
      $scope.data_m = null;
      // Agronomic data
      $scope.data_a = null;
      // Historical data
      $scope.data_h = null;
      // Forecast data
      $scope.data_f = null;
      // Yield crop filtered by weather station
      $scope.yield_ws = null;
      // Yiel ranges for the weather station
      $scope.yield_ranges = null;

      // Form binding
      $scope.agronomic_source = 'cultivar';

      // Load data from web web api
      // Get all geographic data able with information
      GeographicFactory.get().success(function (data_m) {
          $scope.data_m = data_m;

          // Set the first item in the select controls
          $scope.climatology.state = data_m[0];
          $scope.climatology.municipality = data_m[0].municipalities[0];
          $scope.climatology.ws = data_m[0].municipalities[0].weather_stations[0];
          $scope.url.climatology = HistoricalFactory.getUrl($scope.climatology.ws.id);
          //
          $scope.historical_climate.state = data_m[0];
          $scope.historical_climate.municipality = data_m[0].municipalities[0];
          $scope.historical_climate.ws = data_m[0].municipalities[0].weather_stations[0];
          $scope.url.historical_climate = HistoricalFactory.getUrl($scope.historical_climate.ws.id);

          // Load the agronomic information
          AgronomicFactory.get().success(function (data_a) {
              $scope.data_a = data_a;
          }).error(function (error) {
              console.log(error);
          });
      }).error(function (error) {
          console.log(error);
      });

      /*
       * Event to change value in climatology
      */
      $scope.updateClimatology = function () {
          var ws_id = '';
          if ($scope.climatology.ws != undefined)
              ws_id = $scope.climatology.ws.id;
          $scope.url.climatology = HistoricalFactory.getUrl(ws_id);
      }

      /*
       * Event to change value in climatology
      */
      $scope.updateHistoricalClimate = function () {
          var ws_id = '';
          if ($scope.historical_climate.ws != undefined)
              ws_id = $scope.historical_climate.ws.id;
          $scope.url.historical_climate = HistoricalFactory.getUrl(ws_id);
      }


      /*
       * Method that load the data in a table in the webpage
       * (string) source: Type of filter
      */
      $scope.getData = function (source) {
          $(".icon_loading").fadeIn();
          var rows = [];
          if (source === 'geographic') {
              $scope.data_title = 'Datos geográficos';
              $scope.headers = ['country_name', 'state_id', 'state_name', 'municipality_id', 'municipality_name', 'ws_id', 'ws_name', 'ws_origin'];
              for (var i = 0; i < $scope.data_m.length; i++) {
                  var s = $scope.data_m[i];
                  for (var j = 0; j < s.municipalities.length; j++) {
                      var m = s.municipalities[j];
                      for (var k = 0; k < m.weather_stations.length; k++) {
                          var w = m.weather_stations[k];
                          rows.push([s.country, s.id, s.name, m.id, m.name, w.id, w.name, w.origin]);
                      }
                  }
              }
          }
          else if (source === 'agronomic') {
              $scope.data_title = 'Datos agronómicos';
              if ($scope.agronomic_source === 'cultivar') {
                  $scope.headers = ['crop_id', 'crop_name', 'cultivar_id', 'cultivar_name', 'cultivar_rainfed'];
                  for (var i = 0; i < $scope.data_a.length; i++) {
                      var cp = $scope.data_a[i];
                      for (var j = 0; j < cp.cultivars.length; j++) {
                          var cu = cp.cultivars[j];
                          rows.push([cp.cp_id, cp.cp_name, cu.id, cu.name, cu.rainfed]);
                      }
                  }
              }
              else {
                  $scope.headers = ['crop_id', 'crop_name', 'soil_id', 'soil_name'];
                  for (var i = 0; i < $scope.data_a.length; i++) {
                      var cp = $scope.data_a[i];
                      for (var j = 0; j < cp.soils.length; j++) {
                          var so = cp.soils[j];
                          rows.push([cp.cp_id, cp.cp_name, so.id, so.name]);
                      }
                  }
              }
          }
          else if (source === 'climatology') {
              $scope.data_title = 'Climatología';
              $scope.headers = ['ws_id', 'month', 'measure', 'value'];
              HistoricalFactory.get($scope.climatology.ws.id).success(function (data_h) {
                  for (var i = 0; i < data_h.climatology.length; i++) {
                      var w = data_h.climatology[i];
                      for (var j = 0; j < w.monthly_data.length; j++) {
                          var m = w.monthly_data[j];
                          for (var k = 0; k < m.data.length; k++) {
                              var d = m.data[k];
                              rows.push([w.weather_station, m.month, d.measure, d.value]);
                          }
                      }
                  }
              }).error(function (error) {
                  console.log(error);
              });
          }
          else if (source === 'historical_climate') {
              $scope.data_title = 'Histórico climático';
              $scope.headers = ['ws_id', 'year', 'month', 'measure', 'value'];
              HistoricalFactory.get($scope.historical_climate.ws.id).success(function (data_h) {
                  for (var i = 0; i < data_h.climate.length; i++) {
                      var w = data_h.climate[i];
                      for (var j = 0; j < w.monthly_data.length; j++) {
                          var m = w.monthly_data[j];
                          for (var k = 0; k < m.data.length; k++) {
                              var d = m.data[k];
                              rows.push([w.weather_station, w.year, m.month, d.measure, d.value]);
                          }
                      }
                  }
              }).error(function (error) {
                  console.log(error);
              });
          }
          $(".icon_loading").fadeOut();
          $scope.content = rows;
      }

      /*
       * Method that export data to csv file
      */
      $("#exportCsv").click(function () {
          // var outputFile = 'export'
          var outputFile = window.prompt($scope.data_title);
          if (outputFile == null || outputFile === '')
              outputFile = 'export';
          outputFile = outputFile.replace('.csv', '') + '.csv'

          // CSV
          exportTableToCSV.apply(this, [$('#data_raw'), outputFile]);
      });

  });