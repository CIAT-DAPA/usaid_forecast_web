'use strict';

/**
 * @ngdoc function
 * @name ForecastApp.controller:ClimateCtrl
 * @description
 * # ClimateCtrl
 * Controller of the ForecastApp
 */
angular.module('ForecastApp')
  .controller('SubMenuCtrl', function ($scope, $compile, config, tools, $rootScope) {
      
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
          $scope.subSections = [
          {
              name: 'Pronóstico agroclimático', value: 'pronA'
          },
          {
              name: 'Histórico de rendimiento', value: 'hist'
          }
          ];
      }
      else {

      }

      $scope.renderView = function ($value, $name) {
          
          $(".subMenuItem").removeClass("active");
          $("#subMenu-" + $value).addClass("active");
          $(".sections").hide();
          $("#" + $value).show();
          $("#sectionTitle").text($name);
          $rootScope.drawFunction();
          /*var climate_content = $("#containerBlock");
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
                  $compile(climate_content.html());
                  */

      }
  });