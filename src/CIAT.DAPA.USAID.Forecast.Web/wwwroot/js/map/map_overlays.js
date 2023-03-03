/**
  * Method that plots a map with the weather stations
  * @param {any} map leaflet map
  * @param {any} layerConfigs layer params
  */
async function add_map_overlays(map, layerConfigs) {

	var layerControl = new L.control.layers().addTo(map);
	for (const [label, layerName] of Object.entries(layerConfigs)) {
		var owsrootUrl = 'https://geo.aclimate.org/geoserver/administrative/ows';
		var defaultParameters = {
			service: 'WFS',
			version: '1.0.0',
			request: 'GetFeature',
			typeName: 'administrative:' + layerName,
			outputFormat: 'application/json',
		};
		var parameters = L.Util.extend(defaultParameters);
		var URL = owsrootUrl + L.Util.getParamString(parameters);

		$.ajax({
			url: URL,
			success:  (data) =>{
				var geojson = new L.geoJson(data, {
					style: {
						"color": "#22272e",
						"weight": 1.5,
						"opacity": 0.65,
						"fillOpacity": 0
					},
					onEachFeature: function (feature, layer) {
						layer.bindPopup(feature.properties.name_adm2 ?? feature.properties.name_adm1);
						layer.on('mouseover', function () {
							this.setStyle({
								'color': '#3e6db0',
								"weight": 2.5,
								"fillOpacity": 0.1,
							});
						});
						layer.on('mouseout', function () {
							this.setStyle({
								"color": "#22272e",
								"weight": 1.5,
								"fillOpacity": 0,
							});
						});
					}
				});
				layerControl.addOverlay(geojson, label);
			}
		});

		/*
		 * 
		 * OLD WMS
        layers[layerConfigs[i].label] = L.tileLayer.wms('https://geo.aclimate.org/geoserver/administrative/wms?', {
            layers: 'administrative:' + layerConfigs[i].layerName,
            format: 'image/png',
            transparent: true,
            opacity: 0.7,
        });*/
	}



	



}