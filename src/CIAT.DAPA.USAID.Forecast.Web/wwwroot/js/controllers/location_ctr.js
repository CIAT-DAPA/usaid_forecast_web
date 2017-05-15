'use strict';

/**
 * @ngdoc function
 * @name ForecastApp.controller:ClimateCtrl
 * @description
 * # ClimateCtrl
 * Controller of the ForecastApp
 */
angular.module('ForecastApp')
  .controller('LocationCtrl', function ($scope, $window, tools,
                                            GeographicFactory) {
      $scope.type = tools.source();
      $scope.states = null;
      $scope.crop_name = null;

      // Get the municipality from the url
      $scope.municipality_name = tools.search('municipio');

      $(".navbar-default li").removeClass("active");
      if ($scope.type === 'climate') {
          $('#menu_main_climate').addClass('active');
          // Get the geographic information
          GeographicFactory.get().then(
          function (g) {
              $scope.states = g.data;
              // Validate the parameter municipio
              if ($scope.municipality_name == null || $scope.municipality_name === '')
                  $window.location.href = "/Clima?municipio=" + $scope.states[0].municipalities[0].name;
              // Search if the name of parameter exist in the configuration app
              var founded = false;
              for (var i = 0; i < $scope.states.length; i++) {
                  for (var j = 0; j < $scope.states[i].municipalities.length; j++) {
                      if ($scope.states[i].municipalities[j].name.toLowerCase() === $scope.municipality_name.toLowerCase()) {
                          founded = true;
                          break;
                      }
                  }
              }
              // Validate if the municipality was found
              if (!founded)
                  $window.location.href = "/Clima?municipio=" + $scope.states[0].municipalities[0].name;
          },
          function (err) { console.log(err); });
      }
      else if ($scope.type === 'crop') {
          $scope.crop_name = tools.search('cultivo').toLowerCase();

          $('#menu_main_' + ($scope.crop_name === 'arroz' ? 'rice' : 'maize')).addClass('active');

          if ($scope.crop_name !== 'arroz' && $scope.crop_name !== 'maíz')
              $window.location.href = "/Cultivo?cultivo=arroz";

          // Get the geographic information
          GeographicFactory.getByCrop($scope.crop_name).then(
          function (g) {
              $scope.states = g.data;
              // Validate the parameter municipio
              if ($scope.municipality_name == null || $scope.municipality_name === '')
                  $window.location.href = "/Cultivo?municipio=" + $scope.states[0].municipalities[0].name + "&cultivo="+$scope.crop_name;
              // Search if the name of parameter exist in the configuration app
              var founded = false;
              for (var i = 0; i < $scope.states.length; i++) {
                  for (var j = 0; j < $scope.states[i].municipalities.length; j++) {
                      if ($scope.states[i].municipalities[j].name.toLowerCase() === $scope.municipality_name.toLowerCase()) {
                          founded = true;
                          break;
                      }
                  }
              }
              // Validate if the municipality was found
              if (!founded)
                  $window.location.href = "/Cultivo?municipio=" + $scope.states[0].municipalities[0].name + "&cultivo=" + $scope.crop_name;
          },
          function (err) { console.log(err); });

      }
      else {
          $('#menu_main_' + $scope.type).addClass('active');
          $('#mn_municipalities').hide();
      }
  });