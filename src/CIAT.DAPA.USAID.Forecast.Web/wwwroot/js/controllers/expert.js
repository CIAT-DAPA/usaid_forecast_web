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

      $scope.data_title = '';

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
          // List all municipalities
          /*$scope.municipalities = MunicipalityFactory.listByIds(data_m, $scope.gv_municipalities);
          // Search the weather station
          $scope.ws_entity = WeatherStationFactory.getByMunicipality(data_m, $scope.municipality_name);
          // Get yield ranges
          $scope.yield_ranges = WeatherStationFactory.getRanges($scope.ws_entity, $scope.crop_name);
          */
          // Load the agronomic information
          AgronomicFactory.get().success(function (data_a) {
              $scope.data_a = data_a;
          }).error(function (error) {
              console.log(error);
          });
      }).error(function (error) {
          console.log(error);
      });

      $scope.getData = function (source) {
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