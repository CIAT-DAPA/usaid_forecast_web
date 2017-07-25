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
      // Get the source the type of request
      $scope.type = tools.source();
      // Get the municipality from the url
      $scope.municipality_name = tools.search('municipio');
      // Global vars      
      $scope.states_climate = null;
      $scope.states_rice = null;
      $scope.states_maize = null;
      $scope.crop_name = null;

      // Remove active link
      $(".navbar-default li").removeClass("active");

      // Get the geographic information for weather
      GeographicFactory.get().then(function (g) {
          $scope.states_climate = g.data;

          if ($scope.type === 'climate') {
              $('#menu_main_climate').addClass('active');
              // Validate the parameter municipio
              if ($scope.municipality_name == null || $scope.municipality_name === '')
                  $window.location.href = "/Clima?municipio=" + $scope.states_climate[0].municipalities[0].name;
              // Search if the name of parameter exist in the configuration app
              var founded = false;
              for (var i = 0; i < $scope.states_climate.length; i++) {
                  for (var j = 0; j < $scope.states_climate[i].municipalities.length; j++) {
                      if ($scope.states_climate[i].municipalities[j].name.toLowerCase() === $scope.municipality_name.toLowerCase()) {
                          founded = true;
                          break;
                      }
                  }
              }
              // Validate if the municipality was found
              if (!founded)
                  $window.location.href = "/Clima?municipio=" + $scope.states[0].municipalities[0].name;
          }
      },
      function (err) { console.log(err); });
      
      GeographicFactory.getByCrop().then(function (g) {
          // Get the geographic information rice
          $scope.states_rice = g.data.filter(function (item) { return item.name.toLowerCase() === 'arroz'; })[0].states;
          // Get the geographic information maize
          $scope.states_maize = g.data.filter(function (item) { return item.name.toLowerCase() === 'maíz'; })[0].states;

          if ($scope.type === 'crop') {
              $scope.crop_name = tools.search('cultivo').toLowerCase();
              var states = null;
              if($scope.crop_name === 'arroz'){
                  $('#menu_main_rice').addClass('active');
                  states = $scope.states_rice;
              }
              else{
                  $('#menu_main_maize').addClass('active');
                  states = $scope.states_maize;
              }

              if ($scope.crop_name !== 'arroz' && $scope.crop_name !== 'maíz')
                  $window.location.href = "/Cultivo?cultivo=arroz";

              // Validate the parameter municipio
              if ($scope.municipality_name == null || $scope.municipality_name === '')
                  $window.location.href = "/Cultivo?municipio=" + states[0].municipalities[0].name + "&cultivo=" + $scope.crop_name;
              // Search if the name of parameter exist in the configuration app
              var founded = false;
              for (var i = 0; i < states.length; i++) {
                  for (var j = 0; j < states[i].municipalities.length; j++) {
                      if (states[i].municipalities[j].name.toLowerCase() === $scope.municipality_name.toLowerCase()) {
                          founded = true;
                          break;
                      }
                  }
              }
              // Validate if the municipality was found
              if (!founded)
                  $window.location.href = "/Cultivo?municipio=" + $scope.states[0].municipalities[0].name + "&cultivo=" + $scope.crop_name;
          }
      },
      function (err) { console.log(err); });
      
      // Enable active menu
      if ($scope.type !== 'climate' && $scope.type !== 'crop') 
          $('#menu_main_' + $scope.type).addClass('active');
  });