'use strict';

/**
 * @ngdoc function
 * @name ForecastApp.controller:ClimateCtrl
 * @description
 * # ClimateCtrl
 * Controller of the ForecastApp
 */
angular.module('ForecastApp')
  .controller('LocationCtrl', function ($scope, config, tools, GeographicFactory, MunicipalityFactory) {
      $scope.type = tools.source();
      $scope.gv_municipalities = [];

      // Get the municipality from the url
      $scope.municipality_name = tools.search('municipio');
      $scope.crop_name = tools.search('cultivo');

      $(".navbar-default li").removeClass("active");
      if ($scope.type === 'climate') {
          $('#menu_main_climate').addClass('active');
      }
      else if ($scope.type === 'crop') {
          $scope.gv_municipalities = $("#gv_municipalities").val().split(',');
          $('#menu_main_' + (tools.search('cultivo') === 'arroz' ? 'rice' : 'maize')).addClass('active');
      }
      else {
          $('#menu_main_expert').addClass('active');
          $('#mn_municipalities').hide();
      }     
      
      // Vars of the data
      // Data municipalities
      $scope.data_m = null;
      
      // Load data from web web api
      // Get all geographic data able with information
      GeographicFactory.get().success(function (data_m) {
          $scope.data_m = data_m;
          // List all municipalities
          if ($scope.type === 'climate')
              $scope.municipalities = MunicipalityFactory.listAll(data_m);
              
          else if ($scope.type === 'crop')
              $scope.municipalities = MunicipalityFactory.listByIds(data_m, $scope.gv_municipalities);
              
      }).error(function (error) {
          console.log(error);
      });

  });