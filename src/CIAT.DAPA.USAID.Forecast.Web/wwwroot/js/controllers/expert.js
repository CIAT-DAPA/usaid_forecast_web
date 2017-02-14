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

          // Load the agronomic information
          AgronomicFactory.get().success(function (data_a) {
              $scope.data_a = data_a;
              // Load the list of the cultivars and soils from the agronomic configuration
              $scope.cultivars = CultivarsFactory.getByCrop($scope.data_a, $scope.crop_name);
              $scope.soils = SoilFactory.getByCrop($scope.data_a, $scope.crop_name);
              // Load the historical information
              HistoricalFactory.get($scope.ws_entity.id).success(function (data_h) {
                  $scope.data_h = data_h;
                  // Load the Forecast information
                  ForecastFactory.get().success(function (data_f) {
                      $scope.data_f = data_f;
                      // Filter data for weather station
                      $scope.yield_ws = YieldForecastFactory.getByWeatherStation($scope.data_f, $scope.ws_entity.id);
                      $scope.cultivars = CultivarsFactory.getCultivarsAvailableForecast($scope.cultivars, $scope.yield_ws);                      
                  }).error(function (error) {
                      console.log(error);
                  });
              }).error(function (error) {
                  console.log(error);
              });
          }).error(function (error) {
              console.log(error);
          });*/
      }).error(function (error) {
          console.log(error);
      });

      $scope.getData = function (source) {
          var rows = [];
          if (source === 'geographic') {              
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
          $scope.content = rows;
      }
      
  });