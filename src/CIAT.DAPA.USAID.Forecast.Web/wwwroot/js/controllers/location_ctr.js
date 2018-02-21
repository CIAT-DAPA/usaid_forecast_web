'use strict';

/**
 * @ngdoc function
 * @name ForecastApp.controller:ClimateCtrl
 * @description
 * # ClimateCtrl
 * Controller of the ForecastApp
 */
angular.module('ForecastApp')
  .controller('LocationCtrl', function ($scope, $window, tools,
                                            GeographicFactory) {
      // Get the source the type of request
      $scope.type = tools.source();
      // Get the municipality from the url
      $scope.state_name = tools.search(1);
      $scope.municipality_name = tools.search(2);
      $scope.station_name = tools.search(3);
      // Global vars      
      $scope.states_climate = null;
      $scope.states_rice = null;
      $scope.states_maize = null;
      $scope.crop_name = null;
      // Vars for the menu
      $scope.menu_climate = [];
      $scope.menu_rice = [];
      $scope.menu_maize = [];

      // Remove active link
      $(".navbar-default li").removeClass("active");

      // Enable the dropdown list to search data in the menu
      $('#navbar_main  ul  li  a').on('click', function (event) {
          $(this).parent().toggleClass('open');
      });

      // Enable active menu
      if ($scope.type !== 'climate' && $scope.type !== 'crop')
          $('#menu_main_' + $scope.type).addClass('active');

      // Get the geographic information for weather
      GeographicFactory.get().then(function (g) {
          var menu_c = getLocationsToMenu(g.data);

          $scope.menu_climate = menu_c.menu;

          // Transform the select 
          var menu_climate_cbo = $('#menu_climate_cbo').select2({placeholder: 'Seleccione una localidad'});
          menu_climate_cbo.on("change", function (e) {
              $window.location.href = "/Clima/" + $('#menu_climate_cbo').val();
          });

          if ($scope.type === 'climate') {
              $('#menu_main_climate').addClass('active');
              // Validate the parameter municipio
              if ($scope.station_name == null || $scope.station_name === '' || !menu_c.founded)
                  $window.location.href = "/Clima/" + $scope.menu_climate[0].s_name + "/" + $scope.menu_climate[0].m_name + "/" + $scope.menu_climate[0].w_name;
          }
      },
      function (err) { console.log(err); });

      GeographicFactory.getByCrop().then(function (g) {
          // Get the geographic information rice and maize
          var states_rice = getLocationsToMenu(g.data.filter(function (item) { return item.name.toLowerCase() === 'arroz'; })[0].states);
          var states_maize = getLocationsToMenu(g.data.filter(function (item) { return item.name.toLowerCase() === 'maíz'; })[0].states);
          $scope.menu_rice = states_rice.menu;
          $scope.menu_maize = states_maize.menu;
                              
          // Transform the select 
          var menu_rice_cbo = $('#menu_rice_cbo').select2({ placeholder: 'Seleccione una localidad' });
          menu_rice_cbo.on("change", function (e) {
              $window.location.href = "/Cultivo/" + $('#menu_rice_cbo').val() + "/arroz";
          });
          var menu_maize_cbo = $('#menu_maize_cbo').select2({ placeholder: 'Seleccione una localidad' });
          menu_maize_cbo.on("change", function (e) {
              $window.location.href = "/Cultivo/" + $('#menu_maize_cbo').val() + "/maíz";
          });

          if ($scope.type === 'crop') {
              $scope.crop_name = tools.search(4).toLowerCase();
              var states = null;
              if ($scope.crop_name === 'arroz') 
                  $('#menu_main_rice').addClass('active');
              else 
                  $('#menu_main_maize').addClass('active');

              // Validate the parameter municipio
              if ($scope.station_name == null || $scope.station_name === '' || (!states_rice.founded && !states_maize.founded))
                  $window.location.href = "/Cultivo/" + $scope.menu_rice[0].s_name + "/" + $scope.menu_rice[0].m_name + "/" + $scope.menu_rice[0].w_name + "/arroz";
          }
      },
      function (err) { console.log(err); });

      /**
       * Method to transform geographic data
       * (object) data: List data with states
      */
      function getLocationsToMenu(data) {
          var menu = [];
          // Search if the name of parameter exist in the configuration into app
          var founded = false;
          for (var i = 0; i < data.length; i++) {
              for (var j = 0; j < data[i].municipalities.length; j++) {
                  for (var k = 0; k < data[i].municipalities[j].weather_stations.length; k++) {
                      menu.push({ s_id: data[i].id, s_name: data[i].name, m_id: data[i].municipalities[j].id, m_name: data[i].municipalities[j].name, w_id: data[i].municipalities[j].weather_stations[k].id, w_name: data[i].municipalities[j].weather_stations[k].name });
                      if (!founded && $scope.station_name != null && $scope.municipality_name != null && $scope.state_name != null) {
                          founded = data[i].municipalities[j].weather_stations[k].name.toLowerCase() === $scope.station_name.toLowerCase() && data[i].municipalities[j].name.toLowerCase() === $scope.municipality_name.toLowerCase() && data[i].name.toLowerCase() === $scope.state_name.toLowerCase();
                      }   
                  }
              }
          }
          return { menu: menu, founded: founded };
      }

  });