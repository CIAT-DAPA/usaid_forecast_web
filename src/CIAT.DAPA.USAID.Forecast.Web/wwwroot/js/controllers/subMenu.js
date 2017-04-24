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
          $scope.subSections = [
          {
              name: 'Datos geográficos', value: 'content_geographic'
          },
          {
              name: 'Datos agronómicos', value: 'content_agronomic'
          },
          {
              name: 'Climatología', value: 'content_climatology'
          },
          {
              name: 'Hitórico climático', value: 'content_historical_climate'
          },
          {
              name: 'Predicción climática', value: 'content_forecast_climate'
          },
          {
              name: 'Pronóstico de producción', value: 'content_forecast_yield'
          }
          ];
      }

      $scope.renderView = function ($value, $name) {
          
          $(".subMenuItem").removeClass("active");
          $("#subMenu-" + $value).addClass("active");
          $(".sections").hide();
          $("#" + $value).show();
          $("#sectionTitle").text($name);
          $rootScope.drawFunction();
          $("#expert_data article").hide();
      }
  });