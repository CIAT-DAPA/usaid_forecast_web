/**
  * Method that plots a map with the weather stations
  * @param {any} map leaflet map
  * @param {any} layerConfigs layer params
  */
async function add_map_overlays(map, mapOverlays) {

	var layerControl = new L.control.layers().addTo(map);
	mapOverlays.forEach((mapOverlay, index) => { 
		var URL = mapOverlay.src;
		$.ajax({
			url: URL,
			success: (data) => {
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
				layerControl.addOverlay(geojson, mapOverlay.name);
			}
		});
		
})

	







}