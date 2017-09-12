'use strict';

/**
 * @ngdoc function
 * @name ForecastApp.controller:SocialMediaCtrl
 * @description
 * # SocialMediaCtrl
 * Controller of the ForecastApp
 */
angular.module('ForecastApp')
  .controller('AssistCtrl', function ($scope, $rootScope, tools, AssistFactory, TutorialFactory) {
      $scope.type = tools.source();
      $scope.tutorial = false;

      $rootScope.parameters = { id: '', alt: '' };

      // Set the values for the assist (help button)
      if ($scope.type == 'climate')
          $rootScope.parameters = { id: 'forecast', alt: 'forecast' };
      else if ($scope.type == 'crop')
          $rootScope.parameters = { id: 'forecast', alt: 'yield' };
      else if ($scope.type == 'expert')
          $rootScope.parameters = { id: 'expert', alt: '' };
      else if ($scope.type == 'glossary')
          $rootScope.parameters = { id: 'glossary', alt: '' };
      else if ($scope.type == 'about')
          $rootScope.parameters = { id: 'about', alt: '' };
      else
          $rootScope.parameters = { id: 'climate', alt: 'climate' };
      
      /**/
      $rootScope.setAssistParameters = function (id, alt) {
          $rootScope.parameters.id = id;
          $rootScope.parameters.alt = alt;
      }

      /* 
       * Method to show a modal dialog
      */
      $scope.assist = function () {
          var data = AssistFactory.getByIdAlt($rootScope.parameters.id, $rootScope.parameters.alt);
          if (data != null)
              tools.show_assist(data.title, data.text, data.url);
          return false;
      }

      /*
       * Method to show the introduction video
      */
      $rootScope.showTutorial = function () {
          if (TutorialFactory.show_tutorial()) {
              $(".modal_show_tutorial").css('display', 'block');
              tools.show_assist('Bienvenid@', 'En el siguiente video se mostrará un tutorial sobre como usar el sitio web.', 'https://www.youtube.com/embed/S3Hb34Fl0SQ');
              $("#assit_modal").modal();
          }
          return false;
      }

      $scope.click_tutorial = function () {
          TutorialFactory.setShowTutorial(!$scope.tutorial);
          $("#assit_modal").modal('hide');
      }
  });