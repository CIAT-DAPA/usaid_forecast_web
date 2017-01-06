'use strict';

/**
 * @ngdoc function
 * @name ForecastApp.controller:CropCtrl
 * @description
 * # CropCtrl
 * Controller of the ForecastApp
 */
angular.module('ForecastApp')
  .controller('CropCtrl', function ($scope, tools, GeographicFactory, MunicipalityFactory) {
      $scope.crop_name = tools.search('cultivo');
      $scope.municipality_name = tools.search('municipio');
      GeographicFactory.get()
        .success(function (data_g) {
            var m = MunicipalityFactory.listAll(data_g);
        })
        .error(function (error) {
            console.log(error);
        });

  });