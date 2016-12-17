'use strict';

/**
 * @ngdoc function
 * @name ForecastApp.controller:ClimateCtrl
 * @description
 * # ClimateCtrl
 * Controller of the ForecastApp
 */
angular.module('ForecastApp')
  .controller('ClimateCtrl', function ($scope, ForecastFactory, ClimateFactory) {
      var ws = "58504f4f006cb93ed40eec13";
      ForecastFactory.get().success(function (data) {
          var months = ClimateFactory.getProbabilities(data, ws, 'prec');
          var ctrs = '';
          for (var i = 0; i < months.length; i++) {
              var m = months[i];
              ctrs = ctrs + '<article class="col-lg-2 pie_content">' +
                                '<div class="section-content">' +
                                    '<h3>' + m.year + '-' + m.month_name + '</h3>' +
                                    '<div id="pie' + m.year + '-' + m.month + '"></div>' +
                                    '<p class="text-justify">' + m.summary + '</p>' +
                                '</div>' +
                            '</article>';
          }
          $("#probabilities_pies").html(ctrs);
          for (var i = 0; i < months.length; i++) {
              var m = months[i];
              var id = '#pie' + m.year + '-' + m.month;
              var base = new Base(id);
              var pie = new Pie(base);
              pie.render(m.probabilities);
          }
      }).error(function (error) {
          console.log(error);
      });;
  });