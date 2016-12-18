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
          var period = '';
          for (var i = 0; i < months.length; i++) {
              var m = months[i];
              if (i == 0)
                  period = m.month_name + ', ' + m.year + ' a ';
              else if (i == (months.length - 1))
                  period = period + m.month_name + ', ' + m.year;
              ctrs = ctrs + '<article class="col-lg-2 article_content">' +
                                '<div class="section-content">' +
                                    '<h3 class="text-center">Precipitación</h3>' +
                                    '<h4 class="text-center">' + m.year + '-' + m.month_name + '</h4>' +
                                    '<div id="pie' + m.year + '-' + m.month + '"></div>' +
                                    '<p class="text-justify article_content">' + m.summary + '</p>' +
                                '</div>' +
                            '</article>';
          }
          $('#climate-period').html(period);
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