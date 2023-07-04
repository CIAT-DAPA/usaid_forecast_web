

// Global variable for map
var conf;
var layers_all;
var layers_selected;
var maps;
var geoserver_url;
var geoserver_workspace;
var scales;
var categories_title;
var country;
var mapOverlays;

/**
  * Method that plots a map
  * @param {any} id id div
  * @param {any} idx position of the map list
  */
function plot_map(id, idx, min, max, group, type, categories_t, categories_q = [], units) {

    maps[idx] = L.map(id, { zoomControl: false }).setView([conf.latitude, conf.longitude], conf.zoom);
    L.control.zoom({ position: 'bottomright' }).addTo(maps[idx]);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>',
        subdomains: ['a', 'b', 'c']
    }).addTo(maps[idx]);


    //add optional layers
   if (mapOverlays && mapOverlays.length > 0) {
       add_map_overlays(maps[idx], mapOverlays);
    }


   

    var legend = L.control({ position: 'bottomleft' });

    legend.onAdd = function (map) {
        var div = L.DomUtil.create('div', 'info legend');
        div.innerHTML = '<strong>' + categories_t + ' (' + units + ')</strong><br />';
        var categories = type == 'q' ? generatePoints(min, max, 10) : categories_q;


        for (var i = 0; i < categories.length; i++) {
            let next = i < categories.length - 1 ? parseInt(categories[i + 1] - 1) : parseInt(max);
            //console.log(categories_t + ' ' + parseInt(categories[i]) + ',' + next);
            div.innerHTML += '<i class="circle" style="background:' + getColor(i, group) + '"></i> ' +
                (type == 'q' ?
                    parseInt(categories[i]) + (parseInt(categories[i]) == next ? '' : ' - ' + next) :
                    categories[i].id + '- ' + categories[i].description)
                + "<br />";
        }
        return div;
    };
    legend.addTo(maps[idx]);





}

const generatePoints = (startingNumber, endingNumber, maxPoints) => Array.from(
    { length: maxPoints },
    (_, i) => startingNumber + i * parseInt((endingNumber - startingNumber) / (maxPoints - 1))
)

/**
 * Method which loads country boundaries from geojson
 * 
function load_country() {
    $.getJSON("/country.json", function (data) {
        country = data;
    });
}
*/
/**
 * Method that returns the color regarding to index and type
 * @param {any} index
 * @param {any} type
 */
function getColor(index, type) {

    //console.log(index, type, scales);
    return scales[type][index];
}

/**
 * Method which manage the click event into the map
 * @param {any} e latlng (coordinates)
 */
function onMapClick(e) {
    var lat = e.latlng.lat;
    var lon = e.latlng.lng;
    layers_selected.forEach((value, idx) => {

        var marker = L.marker(e.latlng).addTo(maps[idx]);
        searchPointData(geoserver_workspace + ":" + value.cropID + "_" + value.indicatorID, lat, lon, marker, value.units)
    });
}

/**
 * Method that search values into layers to show in a popup message
 * @param {any} layer Layer name
 * @param {any} lat Latitude
 * @param {any} lon Longitud
 * @param {any} marker Marker in which should display the message
 */
function searchPointData(layer, lat, lon, marker, units) {
    const parameters = {
        service: 'WMS',
        version: '1.1.1',
        request: 'GetFeatureInfo',
        layers: layer,
        query_layers: layer,
        feature_count: 10,
        info_format: 'application/json',
        format_options: 'callback:handleJson',
        SrsName: 'EPSG:4326',
        width: 101,
        height: 101,
        x: 50,
        y: 50,
        bbox: (lon - 0.1) + ',' + (lat - 0.1) + ',' + (lon + 0.1) + ',' + (lat + 0.1)
    }
    const url = geoserver_url + new URLSearchParams(parameters).toString();
    axios
        .get(url)
        .then(response => {
            marker.bindPopup("Value: " +
                response.data.features[0].properties.GRAY_INDEX.toFixed(2) +
                " (" + units + ")"
            ).openPopup();
            return response.data;
        });
}

/**
 * Method that plots layers int the map
 * @param {any} idx position of the map
 * @param {any} layer layer to be loaded
 * @param {any} time period of time
 * @param {any} compare period of time
 */
function plot_layer(idx, layer, time, compare) {

    var wmsLayer = L.tileLayer.wms(geoserver_url, {
        layers: geoserver_workspace + ":" + layer,
        format: 'image/png',
        transparent: true,
        // zIndex: 3
    }).addTo(maps[idx]);

    wmsLayer.setParams({ 'time': time });

    // Add a new pane to compare
    if (compare.toLowerCase() != 'none') {
        var wmsLayerCompare = L.tileLayer.wms(geoserver_url, {
            layers: geoserver_workspace + ":" + layer,
            format: 'image/png',
            transparent: true,
            //   zIndex:3
        }).addTo(maps[idx]);

        wmsLayerCompare.setParams({ 'time': compare });

        maps[idx].createPane("left");
        maps[idx].createPane("right");
        L.control.sideBySide(wmsLayer, wmsLayerCompare).addTo(maps[idx]);

        // Add legends
        var legend2 = L.control({ position: 'topright' });

        legend2.onAdd = function (map) {
            var div = L.DomUtil.create('div', 'info legend');
            div.innerHTML = '<strong>' + $("#cbo_compare option:selected").text() + '</strong>';
            return div;
        };
        legend2.addTo(maps[idx]);

        var legend3 = L.control({ position: 'topleft' });

        legend3.onAdd = function (map) {
            var div = L.DomUtil.create('div', 'info legend');
            div.innerHTML = '<strong>' + $("#cbo_time option:selected").text() + '</strong>';
            return div;
        };
        legend3.addTo(maps[idx]);
    }
}

/**
 * Method which updates layers base on user selection
 * */
function update_maps() {
    //console.log("update_maps");
    // Get the values selected in the controls
    var crop = $("#cbo_crop").val();
    var group = $("#cbo_group").val();
    // Filter dataset original    
    layers_selected = layers_all.filter(function (it) {
        return it.cropID == crop &&
            it.groupID == group;
    });
    // Reload maps
    load_maps();
}

function update_time() {
    //console.log("update_time");
    load_maps();
}

/**
 * Method that loads all maps from scratch
 * */
function load_maps() {


    var time = $("#cbo_time").val();
    var compare = $("#cbo_compare").val();
    var maps_section = '';
    layers_selected.forEach((value, idx) => {
        // Condition to validate if start the row 
        if (!(((idx + 1) % 2) == 0))
            maps_section += '<div class="row">';
        // adding a map
        maps_section += '<div class="col-md-6">' +
            '<h3>' + value.indicator + ' (' + value.acronym + ')</h3>' +
            '<p class="text-justify">' + value.description + '</p>' +
            '<div id="map_' + idx + '" class="map_indices"></div>' +
            '</div>';
        // Condition to validate if end the row 
        if (((idx + 1) % 2) == 0 || (idx == (layers_selected.lenght - 1)))
            maps_section += '</div>';

    });
    // Adding HTML
    $("#maps_section").html(maps_section);

    // Clear all maps
    maps = [];
    // Loading maps
    layers_selected.forEach((value, idx) => {
        // ploting maps
        plot_map('map_' + idx, idx, value.min, value.max, value.groupID, value.type, value.acronym, value.categories, value.units);
        // Adding layer
        plot_layer(idx, value.cropID + "_" + value.indicatorID, time, compare);
    });

}
