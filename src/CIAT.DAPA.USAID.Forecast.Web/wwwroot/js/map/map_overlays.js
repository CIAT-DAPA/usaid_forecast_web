/**
  * Method that plots a map with the weather stations
  * @param {any} map leaflet map
  * @param {any} layerConfigs layer params
  */
async function add_map_overlays(map, layerConfigs) {

    var layers = {};
    for (var i = 0; i < layerConfigs.length; i++) {
      
        layers[layerConfigs[i].label] = L.tileLayer.wms('https://geo.aclimate.org/geoserver/administrative/wms?', {
            layers: 'administrative:' + layerConfigs[i].layerName,
            format: 'image/png',
            transparent: true,
            opacity: 0.7,
        });
    }

    L.control.layers(null, layers).addTo(map);
}