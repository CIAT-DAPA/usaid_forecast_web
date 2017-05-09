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
      api_fs_forecast_climate: $('#api_fs_forecast_climate').val(),
      api_fs_forecast_yield: $('#api_fs_forecast_yield').val(),
      api_fs_historical: $('#api_fs_historical').val(),
      api_fs_historical_yield: $('#api_fs_historical_yield').val(),
      api_fs_historical_yield_years: $('#api_fs_historical_yield_years').val(),
      /* Data format */
      float: 0,
      /* Lateral menu */
      sub_menu: {
          climate: [
              { name: 'Predicción climática', section: 'forecast', value: 'forecast' },
              { name: 'Precipitación', section: 'historical', value: 'precipitation' },
              { name: 'Temperatura', section: 'historical', value: 'temperature' },
              { name: 'Radiación solar', section: 'historical', value: 'solar_radiation' }
          ],
          crop: [
              { name: 'Pronóstico agroclimático', value: 'pronA' },
              { name: 'Histórico de rendimiento', value: 'hist' }
          ],
          expert: [
              { name: 'Datos geográficos', value: 'content_geographic' },
              { name: 'Datos agronómicos', value: 'content_agronomic' },
              { name: 'Climatología', value: 'content_climatology' },
              { name: 'Hitórico climático', value: 'content_historical_climate' },
              { name: 'Predicción climática', value: 'content_forecast_climate' },
              { name: 'Pronóstico de producción', value: 'content_forecast_yield' }
          ]
      },
      /* Names in spanish about dates */
      month_names: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
      days_names: ['Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado', 'Domingo'],
      /* Climate vars */
      climate_vars: [
          {
              name: 'Precipitación', container: 'precipitation', value: ['prec'], metric: 'mm', historical_months: [],
              description_climatology: 'Esta gráfica muestra la precipitación promedio ocurrida en los últimos 30 años para cada mes',
              description_historical: ' Esta grafica muestra la precipitación ocurrida para el mismo mes a través de varios años. Para ver el comportamiento histórico mensual de clic sobre el mes de interés'
          },
          {
              name: 'Temperatura', container: 'temperature', value: ['t_max', 't_min'], metric: '°C', historical_months: [],
              description_climatology: 'Esta gráfica muestra la temperatura máxima promedio ocurrida en los últimos 30 años para cada mes',
              description_historical: ' Esta grafica muestra la temperatura máxima ocurrida para el mismo mes a través de varios años. Para ver el comportamiento histórico mensual de clic sobre el mes de interés'

          },
          {
              name: 'Radiación solar', container: 'solar_radiation', value: ['sol_rad'], metric: 'MJ/m²d', historical_months: [],
              description_climatology: 'Esta gráfica muestra la radiación solar promedio ocurrida en los últimos 30 años para cada mes',
              description_historical: ' Esta grafica muestra la radiación solar ocurrida para el mismo mes a través de varios años. Para ver el comportamiento histórico mensual de clic sobre el mes de interés'
          }
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
              crop: "maíz",
              vars: [{ name: "yield_0", label: "Rendimiento", metric: 'Kg/ha', default: true },
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
          { id: 'expert_global', alt: '', title: 'Datos históricos y climáticos', text: 'Datos históricos y climáticos', url: 'https://www.youtube.com/embed/rej55fpq0b8?ecver=1' },
          { id: 'expert_download', alt: '', title: 'Descarga de datos', text: 'Descarga de datos', url: 'https://www.youtube.com/embed/rej55fpq0b8?ecver=1' },
      ]
  })
;