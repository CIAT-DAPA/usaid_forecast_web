'use strict';

/**
 * @ngdoc function
 * @name ForecastApp.controller:ClimateCtrl
 * @description
 * # ClimateCtrl
 * Controller of the ForecastApp
 */
angular.module('ForecastApp')
  .controller('SubMenuCtrl', function ($scope, $rootScope, tools, setup) {
      
      $scope.type = tools.source();

      $scope.subSections = setup.listSubMenuOption($scope.type);      
            
      $scope.renderView = function ($value, $name, $section) {                    
          if ($scope.type === 'climate' || $scope.type === 'crop') {
              console.log($value + ' ' + $section);
              $rootScope.setAssistParameters($section, $value);
              $(".subMenuItem").removeClass("active");
              $("#subMenu-" + $value).addClass("active");
              $(".sections").hide();
              $("#content_" + $section).show();
              $("#sectionTitle").text($name);
              $rootScope.drawFunction($value);              
          }
          else if ($scope.type === 'expert') {
              $rootScope.setAssistParameters($scope.type, '');
              $(".subMenuItem").removeClass("active");
              $("#subMenu-" + $value).addClass("active");
              $("#content_" + $section).show();
              $("#sectionTitle").text($name);
              $rootScope.drawFunction($value);
          }
          else {
              if ($scope.type === 'glossary')
                  $rootScope.setAssistParameters($scope.type, '');
              else ($scope.type === 'about')
                  $rootScope.setAssistParameters($scope.type, '');
              $(".subMenuItem").removeClass("active");
              $("#subMenu-" + $value).addClass("active");
              $(".sections").hide();
              $("#content_" + $section).show();
              $("#sectionTitle").text($name);
          }
      }
  });