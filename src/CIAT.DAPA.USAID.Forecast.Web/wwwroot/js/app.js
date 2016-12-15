'use strict';

/**
 * @ngdoc overview
 * @name ForecastApp
 * @description
 * # ForecastApp
 *
 * Main module of the application.
 */
angular
  .module('ForecastApp', [])
  .value('config', {
      data_clima: 'data/clima/pie.json',
      data_clima_dashboard: 'data/clima/dashboard.json',
      data_arroz: 'data/cultivo/arroz.csv',
      data_arroz_calendar: 'data/cultivo/arroz_calendar.csv',
      data_maiz: 'data/cultivo/maiz.csv',
  });