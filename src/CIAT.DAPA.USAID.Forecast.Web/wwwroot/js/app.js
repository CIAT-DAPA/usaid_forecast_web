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
      api_forecast: 'http://13.92.241.18:8081/api/Forecast',
      api_historical: 'http://13.92.241.18:8081/api/Historical/Get/?weatherstations=',
      month_names: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
      days_names: ['Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado', 'Domingo']
  });