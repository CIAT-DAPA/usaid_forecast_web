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
              { name: 'Pronóstico agroclimático', section: 'forecast', value: 'forecast' },
              { name: 'Histórico agroclimático', section: 'historical', value: 'historical' },
          ],
          expert: [
              { name: 'Datos geográficos', section: 'geographic', value: 'geographic' },
              { name: 'Datos agronómicos', section: 'agronomic', value: 'agronomic' },
              { name: 'Climatología', section: 'climatology', value: 'climatology' },
              { name: 'Histórico climático', section: 'climate_historical', value: 'climate_historical' },
              { name: 'Predicción climática', section: 'climate_forecast', value: 'climate_forecast' },
              { name: 'Histórico de producción', section: 'yield_historical', value: 'yield_historical' },              
              { name: 'Pronóstico de producción', section: 'yield_forecast', value: 'yield_forecast' }
          ],
          about: [
              { name: 'Predicción climatica', section: 'climate', value: 'climate' },
              { name: 'Generación de escenarios', section: 'scenarios', value: 'scenarios' },
              { name: 'Pronóstico agroclimático', section: 'forecast', value: 'forecast' },
              { name: 'Pronóstico agroclimático (arroz)', section: 'yield_rice', value: 'yield_rice' },
              { name: 'Pronóstico agroclimático (maíz)', section: 'yield_maize', value: 'yield_maize' },
              { name: 'Calibración y validación de modelos de cultivos (arroz)', section: 'validation_rice', value: 'validation_rice' },
              { name: 'Calibración y validación de modelos de cultivos (maíz)', section: 'validation_maize', value: 'validation_maize' }
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
              description_historical: ' Esta grafica muestra la precipitación ocurrida para el mismo mes a través de varios años. Para ver el comportamiento histórico mensual seleccione el mes de interés'
          },
          {
              name: 'Temperatura', container: 'temperature', value: ['t_max', 't_min'], metric: '°C', historical_months: [],
              description_climatology: 'Esta gráfica muestra las temperaturas máximas y mínimas promedio ocurrida en los últimos 30 años para cada mes',
              description_historical: ' Esta grafica muestra las temperaturas máximas y mínimas ocurrida para el mismo mes a través de varios años. Para ver el comportamiento histórico mensual seleccione el mes de interés'

          },
          {
              name: 'Radiación solar', container: 'solar_radiation', value: ['sol_rad'], metric: 'MJ/m²d', historical_months: [],
              description_climatology: 'Esta gráfica muestra la radiación solar promedio ocurrida en los últimos 30 años para cada mes',
              description_historical: ' Esta grafica muestra la radiación solar ocurrida para el mismo mes a través de varios años. Para ver el comportamiento histórico mensual seleccione el mes de interés'
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
                  { name: "t_min_acu", label: "T. mínima", metric: '°C', default: false }]
          },
          {
              crop: "maíz",
              vars: [{ name: "yield_0", label: "Rendimiento", metric: 'Kg/ha', default: true },
                  { name: "prec_acu", label: "Precipitación", metric: 'mm', default: false },
                  { name: "t_max_acu", label: "T. máxima", metric: '°C', default: false },
                  { name: "t_min_acu", label: "T. mínima", metric: '°C', default: false },
                  { name: "d_dry", label: "Secado", metric: 'día', default: false },
                  { name: "bio_acu", label: "Biomasa", metric: 'mm', default: false }]
          }],
      /* Expert mode*/
      expert_db: [
          { section: 'geographic', title:'Geografía', description: 'Esta base de datos contiene información sobre la configuración geográfica de los pronósticos y datos históricos que se ofrecen en la plataforma. Es necesaria para poder establecer crear la relación con otras base de datos que están disponibles en esta aplicación.' },
          { section: 'agronomic', title: 'Agronomía', description: 'Esta base de datos contiene información sobre la configuración agronómica de los pronósticos y datos históricos que se ofrecen en la plataforma para los cultivos. Usted puede seleccionar si desea obtener los datos sobre los cultivares de cada cultivo o por el contrario, la configuración de suelo para cada uno de estos.' },
          { section: 'climatology', title: 'Climatología', description: 'En esta base de datos se pueden encontrar filtrados los datos de cada estación climática sobre la climatología. Se pueden obtener los datos de variables como temperaturas máximas y mínimas, precipitación, radiación solar, entre otras. Puede establecer una relación con los datos obtenidos en la sección de datos geográficos.' },
          { section: 'climate_historical', title: 'Histórico Climático', description: 'Usted puede por cada estación climática obtener los datos históricos mensuales que son usados para realizar la predicción climática. Se pueden obtener datos de variables como temperaturas máximas y mínimas, precipitación y radiación solar de cada mes durante un intervalo de tiempo.' },
          { section: 'climate_forecast', title: 'Predicción climática', description: 'Esta base de datos contiene el resultado sobre la última predicción climática realizada para diferentes zonas geográficas. Las probabilidades de precipitación por cada estación climática está dada por la normalidad, es decir, si está por debajo, dentro o por encima del intervalo normal. Tambien puede descargar los escenarios climáticos según su selección.' },
          { section: 'yield_historical', title: 'Histórico de producción', description: 'Esta base de datos contiene el resultado de los datos de rendimiento de los diferentes cultivos en distintos municipios del país.' },
          { section: 'yield_forecast', title: 'Pronóstico agroclimático', description: 'Esta base de datos tiene la información del último pronóstico agroclimático. Esta contiene los datos de producción organizados por estación climática, cultivar y suelo. Los estadistícos son el resultado del modelo del cultivo, ejecutado bajo varios escenarios climáticos.' }
      ],
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