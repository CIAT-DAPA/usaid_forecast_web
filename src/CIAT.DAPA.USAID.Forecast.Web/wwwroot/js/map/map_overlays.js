/**
  * Method that plots a map with the weather stations
  * @param {any} map leaflet map
  * @param {any} layerConfigs layer params
  */
async function addGeoJSONOverlays(map, mapOverlays) {
	try {
		let promises = mapOverlays.map((mapOverlay) => downloadGeoJSON(mapOverlay.src))
		let dataSets = await Promise.all(promises);
		let layers = dataSets.map(data => convertToGeoJSON(data));
		var layerControl = new L.control.layers().addTo(map);

		layers.forEach((layer, i) => {
			layerControl.addOverlay(layer, mapOverlays[i].name);
		})

	} catch (error) {
		console.error(error)
	}
}


// load geojson
async function downloadGeoJSON(url) {
	var res = await axios.get(url);
	return res.data;
}
function convertToGeoJSON(data) {
	return new L.geoJson(data, {
		style: {
			"color": "#22272e",
			"weight": 1.5,
			"opacity": 0.65,
			"fillOpacity": 0
		},
		pane:'right',
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

}