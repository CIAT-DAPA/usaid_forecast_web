﻿'use strict';

/**
 * @ngdoc function
 * @name ForecastApp.controller:CropCtrl
 * @description
 * # CropCtrl
 * Controller of the ForecastApp
 */
angular.module('ForecastApp')
  .controller('ExpertCtrl', function ($rootScope, $scope, $window, setup, tools,
                                    GeographicFactory, ForecastApiFactory) {
      //Url
      $scope.url_json = null;
      $scope.url_csv = null;
      // Var data
      $scope.data_m = null;
      // Databases
      $scope.db = setup.getExpertDatabases();
      $scope.db_selected = $scope.db[0];
      // Filters
      $scope.show_agronomic = false;
      $scope.show_geographic = false;
      $scope.show_probabilities = false;
      // Controls
      $scope.agronomic_source = 'true'
      $scope.probabilities = 'true'
      $scope.state = null;
      $scope.municipality = null;
      $scope.ws = null;


      load_data();

      function load_data() {
          // Load data from web web api
          // Get all geographic data able with information
          GeographicFactory.db.init(true, "json");
          GeographicFactory.get().then(
          function (result) {
              $scope.data_m = result.data;
              $scope.state = $scope.data_m[0];
              $scope.municipality = $scope.data_m[0].municipalities[0];
              $scope.ws = $scope.data_m[0].municipalities[0].weather_stations[0];
              $rootScope.drawFunction($scope.db_selected.section);
          },
          function (error) { console.log(error); });
      }

      /*rootscope function*/
      $rootScope.drawFunction = function (section) {
          $scope.db_selected = $scope.db.filter(function (item) { return item.section === section; })[0];
          $scope.show_agronomic = false;
          $scope.show_geographic = false;
          $scope.show_probabilities = false;

          if (section === 'geographic') {
              $scope.load_geographic();
          }
          else if (section === 'agronomic') {
              $scope.load_agronomic();
          }
          else if (section === 'climatology') {
              $scope.load_climatology();
          }
          else if (section === 'climate_historical') {
              $scope.load_climate_historical();
          }
          else if (section === 'climate_forecast') {
              $scope.load_climate_forecast();
          }
      }

      $scope.load_geographic = function() {
          ForecastApiFactory.init(true, "json");
          $scope.url_json = ForecastApiFactory.getUrlGeographic();
          ForecastApiFactory.init(true, "csv");
          $scope.url_csv = ForecastApiFactory.getUrlGeographic();
      }

      $scope.load_agronomic = function() {
          $scope.show_agronomic = true;
          ForecastApiFactory.init(true, "json");
          $scope.url_json = ForecastApiFactory.getUrlAgronomic($scope.agronomic_source);
          ForecastApiFactory.init(true, "csv");
          $scope.url_csv = ForecastApiFactory.getUrlAgronomic($scope.agronomic_source);
      }

      $scope.load_climatology = function () {
          $scope.show_geographic = true;
          if ($scope.ws != undefined) {
              ForecastApiFactory.init(true, "json");
              $scope.url_json = ForecastApiFactory.getUrlHistoricalClimatology($scope.ws.id);
              ForecastApiFactory.init(true, "csv");
              $scope.url_csv = ForecastApiFactory.getUrlHistoricalClimatology($scope.ws.id);
          }
      }

      $scope.load_climate_historical = function () {
          $scope.show_geographic = true;
          if ($scope.ws != undefined) {
              ForecastApiFactory.init(true, "json");
              $scope.url_json = ForecastApiFactory.getUrlHistoricalClimate($scope.ws.id);
              ForecastApiFactory.init(true, "csv");
              $scope.url_csv = ForecastApiFactory.getUrlHistoricalClimate($scope.ws.id);
          }
      }

      $scope.load_climate_forecast = function () {
          $scope.show_probabilities = true;
          $scope.show_geographic = true;
          if ($scope.ws != undefined) {
              ForecastApiFactory.init(true, "json");
              $scope.url_json = ForecastApiFactory.getUrlClimate($scope.ws.id,$scope.probabilities);
              ForecastApiFactory.init(true, "csv");
              $scope.url_csv = ForecastApiFactory.getUrlClimate($scope.ws.id, $scope.probabilities);
          }
      }

      $scope.update_geographic = function (section) {
          if (section === 'climatology') {
              $scope.load_climatology();
          }
          else if (section === 'climate_historical') {
              $scope.load_climate_historical();
          }
      }

      /*
       * Method that redir to user to download data
       * (string) url: Url to redir
      */
      $scope.getData = function (url) {
          $window.open(url, '_blank');
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

  });