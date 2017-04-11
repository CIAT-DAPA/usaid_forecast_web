'use strict';

/**
 * @ngdoc function
 * @name ForecastApp.controller:ClimateCtrl
 * @description
 * # ClimateCtrl
 * Controller of the ForecastApp
 */
angular.module('ForecastApp')
  .controller('SubMenuCtrl', function ($scope, config,tools) {
      
      $scope.type = tools.source();

      if ($scope.type === 'climate') {
          $scope.subSections = [
          {
              name: 'Predicción climática', value: 'prob'
          },
          {
              name: 'Precipitación', value: 'prec'
          },
          {
              name: 'Temperatura', value: 't_max'
          },
          {
              name: 'Radiación solar', value: 'sol_rad'
          }
          ];
      }
      else if ($scope.type === 'crop') {
          $scope.subSections = config.yield_default_var;
      }
      else {

      }

      $scope.renderView = function ($value, $index) {
          console.log($value);
          console.log($index);
          var climate_content = $("#containerBlock");
          if ($value === 'prob') {
              $(".sectionTitle").text("Predicción Climática");
              $.get('/Clima/Forecast', function (data) {
                  climate_content.html(data);
              });
          }
          else {
              $.get('/Clima/ClimateVars', function (data) {
                  climate_content.html(data);
              });

          }
      }
  });