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

      var parameters = { id: '', alt: '' };

      // Set the values for the assist (help button)
      if ($scope.type == 'climate')
          parameters = { id: 'forecast', alt: 'forecast' };
      else if ($scope.type == 'crop')
          parameters = { id: 'forecast', alt: 'yield' };
      else if ($scope.type == 'expert')
          parameters = { id: '', alt: '' };
      else
          parameters = { id: 'climate', alt: 'climate' };
      
      /**/
      $rootScope.setAssistParameters = function (id, alt) {
          parameters.id = id;
          parameters.alt = alt;
      }

      /* 
       * Method to show a modal dialog
      */
      $scope.assist = function () {
          var data = AssistFactory.getByIdAlt(parameters.id, parameters.alt);
          if (data != null)
              tools.show_assist(data.title, data.text, data.url);
          return false;
      }

      /*
       * 
      */
      $rootScope.showTutorial = function () {          
          if (TutorialFactory.show_tutorial()) {              
              tools.show_assist('Bienvenid@', 'En el siguiente video se mostrará un tutorial sobre como usar el sitio web.', 'https://www.youtube.com/embed/8ncXEbYGHrU');
              $("#assit_modal").modal();
          }
          return false;
      }


      

  });