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
      api_fs: $('#api_fs').val(),
      api_fs_geographic: $('#api_fs_geographic').val(),
      api_fs_agronomic: $('#api_fs_agronomic').val(),
      api_fs_forecast: $('#api_fs_forecast').val(),
      api_fs_historical: $('#api_fs_historical').val(),
      month_names: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
      days_names: ['Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado', 'Domingo'],
      climate_vars: [
          { name: 'Precipitación', value: 'prec', metric: 'mm', historical_months: [] },
          { name: 'Temperatura máxima', value: 't_max', metric: '°C', historical_months: [] },
          { name: 'Temperatura minima', value: 't_min', metric: '°C', historical_months: [] },
          { name: 'Radiación solar', value: 'sol_rad', metric: 'MJ/m²d', historical_months: [] }
      ],
      yield_default_var: [
          {
              crop: "arroz", vars:
                  [{ name: "yield_14", label: "Rendimiento", metric: 'Kg/héc', default: true },
                   { name: "d_har", label: "Cosecha", metric: 'día', default: false },
                  { name: "prec_acu", label: "Precipitación", metric: 'mm', default: false },
                  { name: "t_max_acu", label: "T. máxima", metric: '°C', default: false },
                  { name: "t_min_acu", label: "T. mínima", metric: '°C', default: false }]
          },
          {
              crop: "maiz", vars:
                  [{ name: "yield_0", label: "Rendimiento", metric: 'Kg/héc', default: true },
                   { name: "d_har", label: "Cosecha", metric: 'día', default: false },
                  { name: "prec_acu", label: "Precipitación", metric: 'mm', default: false },
                  { name: "t_max_acu", label: "T. máxima", metric: '°C', default: false },
                  { name: "t_min_acu", label: "T. mínima", metric: '°C', default: false },
                  { name: "d_dry", label: "Secado", metric: 'día', default: false },
                  { name: "bio_acu", label: "Biomasa", metric: 'mm', default: false }]
          }]
  })
  .factory('tools', function () {
      var _tools = {};
      _tools.search = function (name) {
          var url = window.location.href;
          name = name.replace(/[\[\]]/g, "\\$&");
          var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"), results = regex.exec(url);
          if (!results) return null;
          if (!results[2]) return '';
          return decodeURIComponent(results[2].replace(/\+/g, " "));
      }
      return _tools;
  });