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
  .module('ForecastApp', ['ngCookies'])
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
              { name: 'Pronóstico agroclimático', section: 'forecast', value: 'yield' },
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
          glossary: [
              { name: 'Biomasa acumulada', section: 'bio_acu', value: 'bio_acu' },
              { name: 'Días a secado', section: 'd_dry', value: 'd_dry' },
              { name: 'Días a cosecha', section: 'd_har', value: 'd_har' },
              { name: 'Evapotranspiración', section: 'eva', value: 'eva' },
              { name: 'Intervalo de confianza', section: 'conf_int', value: 'conf_int' },
              { name: 'Precipitación', section: 'prec', value: 'prec' },
              { name: 'Precipitación acumulada', section: 'prec_acu', value: 'prec_acu' },
              { name: 'Predicción climática', section: 'climate', value: 'climate' },
              { name: 'Promedio histórico', section: 'pro_his', value: 'pro_his' },
              { name: 'Pronóstico agroclimático', section: 'forecast', value: 'forecast' },
              { name: 'Radiación solar', section: 'sol_rad', value: 'sol_rad' },
              { name: 'Rendimiento', section: 'yield', value: 'yield' },
              { name: 'Rendimiento potencial', section: 'yield_pot', value: 'yield_pot' },
              { name: 'Temperatura máxima', section: 't_max', value: 't_max' },
              { name: 'Temperatura máxima acumulada', section: 't_max_acu', value: 't_max_acu' },
              { name: 'Temperatura mínima', section: 't_min', value: 't_min' },
              { name: 'Temperatura mínima acumulada', section: 't_min_acu', value: 't_min_acu' },
          ],
          about: [
              { name: 'Generación de escenarios', section: 'scenarios', value: 'scenarios' },
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
              description_climatology: 'Esta gráfica muestra la precipitación promedio de los últimos 30 años para cada mes',
              description_historical: ' Esta gráfica muestra los valores históricos de la precipitación para un solo mes. Seleccione el mes de interés'
          },
          {
              name: 'Temperatura', container: 'temperature', value: ['t_max', 't_min'], metric: '°C', historical_months: [],
              description_climatology: 'Esta gráfica muestra las temperaturas máximas y mínimas promedio de los últimos 30 años para cada mes',
              description_historical: ' Esta gráfica muestra los valores históricos de las temperaturas máximas y mínimas para un solo mes. Seleccione el mes de interés'

          },
          {
              name: 'Radiación solar', container: 'solar_radiation', value: ['sol_rad'], metric: 'MJ/m²d', historical_months: [],
              description_climatology: 'Esta gráfica muestra la radiación solar promedio de los últimos 30 años para cada mes',
              description_historical: ' Esta gráfica muestra los valores históricos de la radiación solar para un solo mes. Seleccione el mes de interés'
          }
      ],
      climate_vars_scenario: [
          { name: 'Precipitación', value: 'prec', metric: 'mm' },
          { name: 'T. Máxima', value: 't_max', metric: '°C' },
          { name: 'T. Mínima', value: 't_min', metric: '°C' },
          { name: 'Radiación solar', value: 'sol_rad', metric: 'MJ/m²d' }
      ],
      climatology_forecast: { lower: 'prec_ter_1', upper: 'prec_ter_2' },
      /* Yield vars */
      yield_default_var: [
          {
              crop: "arroz",
              vars: [{ name: "yield_14", label: "Rendimiento", description: 'Rendimiento al 14% de húmedad', metric: 'Kg/ha', default: true },
                   { name: "d_har", label: "D. Cosecha", description: 'Días a cosecha', metric: 'día(s)', default: false },
                  { name: "prec_acu", label: "Precipitación", description: 'Precipitación acumulada', metric: 'mm', default: false },
                  { name: "t_max_acu", label: "T. máxima", description: 'Temperatura máxima acumulada', metric: '°C', default: false },
                  { name: "t_min_acu", label: "T. mínima", description: 'Temperatura mínima acumulada', metric: '°C', default: false },
                  //{ name: "et_acu", label: "Evapotranspiración", description: 'Evapotranspiración acumulada', metric: 'mm', default: false }
              ]
          },
          {
              crop: "maíz",
              vars: [{ name: "yield_0", label: "Rendimiento", description: 'Rendimiento al 0% de húmedad', metric: 'Kg/ha', default: true },
                  { name: "prec_acu", label: "Precipitación", description: 'Precipitación acumulada', metric: 'mm', default: false },
                  { name: "t_max_acu", label: "T. máxima", description: 'Temperatura máxima acumulada', metric: '°C', default: false },
                  { name: "t_min_acu", label: "T. mínima", description: 'Temperatura mínima acumulada', metric: '°C', default: false },
                  { name: "d_dry", label: "D. Secado", description: 'Días a secado', metric: 'día(s)', default: false },
                  { name: "bio_acu", label: "Biomasa", description: 'Biomasa acumulada', metric: 'mm', default: false },
                  //{ name: "et_acu", label: "Evapotranspiración", description: 'Evapotranspiración acumulada', metric: 'mm', default: false }
              ]
          }],
      /* Expert mode*/
      expert_db: [
          { section: 'geographic', title: 'Datos geográficos', description: 'Esta base de datos contiene información sobre los estados, municipios y estaciones climáticas que se encuentran disponibles para los pronósticos y datos históricos que se ofrecen en la plataforma. Es necesaria para poder establecer crear la relación con otras base de datos de esta aplicación.' },
          { section: 'agronomic', title: 'Datos agronómicos', description: 'Esta base de datos contiene información sobre la configuración agronómica de los pronósticos y datos históricos que se ofrecen en la plataforma para los cultivos. Usted puede seleccionar si desea obtener los datos sobre los cultivares de cada cultivo o por el contrario, la configuración de suelo para cada uno de estos.' },
          { section: 'climatology', title: 'Climatología', description: 'En esta base de datos se pueden encontrar filtrados los datos de cada estación climática sobre la climatología. Se pueden obtener los datos de variables como temperaturas máximas y mínimas, precipitación, radiación solar, entre otras. Puede establecer una relación con los datos obtenidos en la sección de datos geográficos.' },
          { section: 'climate_historical', title: 'Histórico Climático', description: 'Usted puede por cada estación climática obtener los datos históricos mensuales que son usados para realizar la predicción climática. Se pueden obtener datos de variables como temperaturas máximas y mínimas, precipitación y radiación solar de cada mes durante un intervalo de tiempo.' },
          { section: 'climate_forecast', title: 'Histórico climático', description: 'Esta base de datos contiene el resultado sobre la última predicción climática realizada para diferentes zonas geográficas. Las probabilidades de precipitación por cada estación climática está dada por la normalidad, es decir, si está por debajo, dentro o por encima del intervalo normal. Tambien puede descargar los escenarios climáticos según su selección.' },
          { section: 'yield_historical', title: 'Histórico de producción', description: 'Esta base de datos contiene el resultado de los datos de rendimiento de los diferentes cultivos en distintos municipios del país.' },
          { section: 'yield_forecast', title: 'Pronóstico de producción', description: 'Esta base de datos tiene la información del último pronóstico agroclimático. Esta contiene los datos de producción organizados por estación climática, cultivar y suelo. Los estadistícos son el resultado del modelo del cultivo, ejecutado bajo varios escenarios climáticos.' }
      ],
      /* Assist data */
      assist: [
          { id: 'forecast', alt: 'forecast', title: 'Predicción climática', text: 'Predicción climatica', url: 'https://www.youtube.com/embed/8ncXEbYGHrU' },
          { id: 'forecast', alt: 'yield', title: 'Pronóstico agroclimatico', text: 'Pronóstico agroclimatico', url: 'https://www.youtube.com/embed/okIT7QZLHss' },
          { id: 'historical', alt: 'precipitation', title: 'Precipitación Histórica', text: 'Precipitación Histórica', url: 'https://www.youtube.com/embed/Ag3fv67zr88' },
          { id: 'climate', alt: 'climate', title: 'Acerca de', text: 'Acerca de', url: 'https://www.youtube.com/embed/c03280UMJoU' },
      ]
  })
;