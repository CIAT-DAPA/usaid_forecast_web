'use strict';

/**
 * @ngdoc function
 * @name ForecastApp.controller:ClimateCtrl
 * @description
 * # ClimateCtrl
 * Controller of the ForecastApp
 */
angular.module('ForecastApp')
  .controller('LocationCtrl', function ($scope, $window, tools, GeographicFactory) {
      $scope.type = tools.source();
      $scope.states = null;

      // Get the municipality from the url
      $scope.municipality_name = tools.search('municipio');
      $scope.crop_name = tools.search('cultivo');

      $(".navbar-default li").removeClass("active");
      if ($scope.type === 'climate') {
          $('#menu_main_climate').addClass('active');
          GeographicFactory.get().then(
              function (s) {
                  $scope.states = s.data;

                  if ($scope.municipality_name == null || $scope.municipality_name === '')
                      $window.location.href = "/Clima?municipio=" + $scope.states[0].municipalities[0].name;

                  var founded = false;
                  for (var i = 0; i < $scope.states.length; i++) {
                      for (var j = 0; j < $scope.states[i].municipalities.length; j++) {
                          if ($scope.states[i].municipalities[j].name.toLowerCase() === $scope.municipality_name.toLowerCase()) {
                              founded = true;
                              break;
                          }
                      }
                  }

                  if (!founded)
                      $window.location.href = "/Clima?municipio=" + $scope.states[0].municipalities[0].name;

              },
              function (err) {
              });
      }
      else if ($scope.type === 'crop') {
          $scope.gv_municipalities = $("#gv_municipalities").val().split(',');
          $('#menu_main_' + ($scope.crop_name === 'arroz' ? 'rice' : 'maize')).addClass('active');
      }
      else {
          $('#menu_main_expert').addClass('active');
          $('#mn_municipalities').hide();
      }
  });