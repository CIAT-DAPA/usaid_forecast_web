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
      /* Web API Url */
      api_fs: $('#api_fs').val(),
      api_fs_geographic: $('#api_fs_geographic').val(),
      api_fs_agronomic: $('#api_fs_agronomic').val(),
      api_fs_forecast: $('#api_fs_forecast').val(),
      api_fs_historical: $('#api_fs_historical').val(),
      api_fs_historical_yield: $('#api_fs_historical_yield').val(),
      api_fs_historical_yield_years: $('#api_fs_historical_yield_years').val(),
      /* Names in spanish about dates */
      month_names: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
      days_names: ['Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado', 'Domingo'],
      /* Climate vars */
      climate_vars: [
          { name: 'Precipitación', value: 'prec', metric: 'mm', historical_months: [] },
          { name: 'Temperatura máxima', value: 't_max', metric: '°C', historical_months: [] },
          { name: 'Temperatura minima', value: 't_min', metric: '°C', historical_months: [] },
          { name: 'Radiación solar', value: 'sol_rad', metric: 'MJ/m²d', historical_months: [] }
      ],
      climatology_forecast: { lower: 'prec_ter_1', upper: 'prec_ter_2' },
      /* Yield vars */
      yield_default_var: [
          {
              crop: "arroz",
              vars: [{ name: "yield_14", label: "Rendimiento", metric: 'Kg/ha', default: true },
                   { name: "d_har", label: "Cosecha", metric: 'día', default: false },
                  { name: "prec_acu", label: "Precipitación", metric: 'mm', default: false },
                  { name: "t_max_acu", label: "T. máxima", metric: '°C', default: false },
                  { name: "t_min_acu", label: "T. mínima", metric: '°C', default: false }],
              guild: { name: "FEDEARROZ" },
              model: { name: "Oryza 2000" }
          },
          {
              crop: "maiz",
              vars: [{ name: "yield_0", label: "Rendimiento", metric: 'Kg/ha', default: true },
                   { name: "d_har", label: "Cosecha", metric: 'día', default: false },
                  { name: "prec_acu", label: "Precipitación", metric: 'mm', default: false },
                  { name: "t_max_acu", label: "T. máxima", metric: '°C', default: false },
                  { name: "t_min_acu", label: "T. mínima", metric: '°C', default: false },
                  { name: "d_dry", label: "Secado", metric: 'día', default: false },
                  { name: "bio_acu", label: "Biomasa", metric: 'mm', default: false }],
              guild: { name: "FENALCE" },
              model: { name: "DSSAT" }
          }],
      /* Assist data */
      assist: [
          { id: 'climate_forecast', alt: '', title: 'Predicción climática', text: 'Predicción climatica', url: 'https://www.youtube.com/embed/rej55fpq0b8?ecver=1' },
          { id: 'climate_var', alt: 'prec', title: 'Precipitación', text: 'Precipitación', url: 'https://www.youtube.com/embed/rej55fpq0b8?ecver=1' },
          { id: 'climate_var', alt: 't_max', title: 'Temperatura máxima', text: 'Temperatura máxima', url: 'https://www.youtube.com/embed/rej55fpq0b8?ecver=1' },
          { id: 'climate_var', alt: 't_min', title: 'Temperatura mínima', text: 'Temperatura mínima', url: 'https://www.youtube.com/embed/rej55fpq0b8?ecver=1' },
          { id: 'climate_var', alt: 'sol_rad', title: 'Radiación solar', text: 'Radiación solar', url: 'https://www.youtube.com/embed/rej55fpq0b8?ecver=1' },
          { id: 'climate_climatology', alt: '', title: 'Climatología', text: 'Climatología', url: 'https://www.youtube.com/embed/rej55fpq0b8?ecver=1' },
          { id: 'climate_historical', alt: '', title: 'Histórico climático', text: 'Histórico climático', url: 'https://www.youtube.com/embed/rej55fpq0b8?ecver=1' },
          { id: 'crop_forecast', alt: '', title: 'Pronóstico agroclimatico', text: 'Pronóstico agroclimatico', url: 'https://www.youtube.com/embed/rej55fpq0b8?ecver=1' },
          { id: 'crop_cultivar', alt: '', title: 'Cultivar', text: 'Cultivar', url: 'https://www.youtube.com/embed/rej55fpq0b8?ecver=1' },
          { id: 'crop_potential', alt: '', title: 'Rendimiento potencial', text: 'Rendimiento potencial', url: 'https://www.youtube.com/embed/rej55fpq0b8?ecver=1' },
          { id: 'crop_variation', alt: '', title: 'Análisis de cultivar', text: 'Análisis de cultivar', url: 'https://www.youtube.com/embed/rej55fpq0b8?ecver=1' },
          { id: 'crop_historical', alt: '', title: 'Histórico de rendimientos', text: 'Histórico de rendimientos', url: 'https://www.youtube.com/embed/rej55fpq0b8?ecver=1' },
      ]
  })
  .factory('tools', function () {
      var _tools = {};

      /*
       * Method that get the value of the parameter from url
       * (string) name: Parameter name
      */
      _tools.search = function (name) {
          var url = window.location.href;
          name = name.replace(/[\[\]]/g, "\\$&");
          var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"), results = regex.exec(url);
          if (!results) return null;
          if (!results[2]) return '';
          return decodeURIComponent(results[2].replace(/\+/g, " "));
      }

      /*
       * Method to determinate which is the current webpage
      */
      _tools.source = function () {          
          if (window.location.href.includes('Clima') || window.location.href.includes('clima'))
              return 'climate';
          else if (window.location.href.includes('Cultivo') || window.location.href.includes('cultivo'))
              return 'crop';
          else
              return 'expert';
      }

      /*
       * Method that changes the content of the modal window
       * (string) title: Title of the modal window
       * (string) text: Content in the modal window
       * (string) url: Url to see a video, if it is null or empty, the video won't show
      */
      _tools.show_assist = function (title, text, url) {
          $("#modal_title").html(title);
          $("#modal_text").html(text);
          if (url != null && url !== '')
              $("#modal_video").attr("src", url);
          else
              $("#modal_video").remove();
      }

      return _tools;
  });