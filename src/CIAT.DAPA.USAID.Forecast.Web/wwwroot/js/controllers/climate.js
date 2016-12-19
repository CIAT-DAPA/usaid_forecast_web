'use strict';

/**
 * @ngdoc function
 * @name ForecastApp.controller:ClimateCtrl
 * @description
 * # ClimateCtrl
 * Controller of the ForecastApp
 */
angular.module('ForecastApp')
  .controller('ClimateCtrl', function ($scope, HistoricalFactory, ClimatologyFactory, HistoricalClimateFactory, ForecastFactory, ClimateFactory) {
      var ws = "58504f4f006cb93ed40eec13";
      var gv_months = $("#gv_months").val().split(',');
      // Load the historical information
      HistoricalFactory.get(ws).success(function (data_h) {
          // Climatology
          var climatology = ClimatologyFactory.getMonthlyData(data_h, ws, gv_months, 'prec');
          var base = new Base('#bar_climatology');
          base.setMargin(10, 30, 10, 10);
          var bar = new Bars(base);
          bar.render(climatology);

          // Historical
          var historical = HistoricalClimateFactory.getData(data_h, ws, 1, 'prec');
          var base = new Base('#line_historical');
          base.setMargin(10, 30, 10, 10);
          var line = new Line(base);
          line.render(historical);

          // Load the Forecast information
          ForecastFactory.get().success(function (data_f) {
              // Climate
              var months = ClimateFactory.getProbabilities(data_f, ws, 'prec');
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
          });
      }).error(function (error) {
          console.log(error);
      });
  });