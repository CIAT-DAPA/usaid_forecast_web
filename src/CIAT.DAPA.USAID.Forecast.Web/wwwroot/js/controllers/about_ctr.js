angular.module('ForecastApp')
  .controller('AboutCtrl', function ($rootScope) {
      // Close loading 
      window.loading_screen.finish();
      // Show tutorial
      $rootScope.showTutorial();

      /*
       * Method that render the data in the screen
       * (string) section: Section name to draw
      */
      $rootScope.drawFunction = function (section) {

      }
  });