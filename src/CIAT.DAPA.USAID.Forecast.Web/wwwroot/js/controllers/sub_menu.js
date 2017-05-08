'use strict';

/**
 * @ngdoc function
 * @name ForecastApp.controller:ClimateCtrl
 * @description
 * # ClimateCtrl
 * Controller of the ForecastApp
 */
angular.module('ForecastApp')
  .controller('SubMenuCtrl', function ($scope, $compile, config, tools, $rootScope, setup) {
      
      $scope.type = tools.source();

      if ($scope.type === 'climate') 
          $scope.subSections = setup.listSubMenuOption('climate');
      else if ($scope.type === 'crop') 
          $scope.subSections = setup.listSubMenuOption('crop');
      else
          $scope.subSections = setup.listSubMenuOption('expert');

      $scope.renderView = function ($value, $name) {
          
          $(".subMenuItem").removeClass("active");
          $("#subMenu-" + $value).addClass("active");
          $(".sections").hide();
          $("#content_" + $value).show();
          $("#sectionTitle").text($name);
          $rootScope.drawFunction($value);
          //$("#expert_data article").hide();
      }
  });