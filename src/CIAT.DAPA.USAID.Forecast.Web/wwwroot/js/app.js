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
      api_forecast: 'http://localhost:59292/api/Forecast',
      month_names: ['Enero','Febrero','Marzo','Abril','Mayo','Junio','Julio','Agosto','Septiembre','Octubre','Noviembre','Diciembre']
  });