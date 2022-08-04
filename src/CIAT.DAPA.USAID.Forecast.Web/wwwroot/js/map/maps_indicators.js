﻿// Global variable for map
var conf;
var layers_all;
var layers_selected;
var maps;
var geoserver_url;
var geoserver_workspace;
var scales;
var categories_title;

/**
  * Method that plots a map
  * @param {any} id id div
  * @param {any} idx position of the map list
  */
function plot_map(id, idx, min, max, group, type, categories_q = []) {
    maps[idx] = L.map(id).setView([conf.latitude, conf.longitude], conf.zoom);
    L.tileLayer('http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>',
        subdomains: ['a', 'b', 'c']
    }).addTo(maps[idx]);

    // Adding event click on map. It shows the value of each pixel
    maps[idx].on('click', onMapClick);

    // 
    var legend = L.control({ position: 'bottomleft' });

    legend.onAdd = function (map) {
        var div = L.DomUtil.create('div', 'info legend');
        div.innerHTML = '<strong>' + categories_title + '</strong><br />';
        var categories = type == 'q' ? generatePoints(min, max, 10) : categories_q;

        
        for (var i = 0; i < categories.length; i++) {
            div.innerHTML += '<i class="circle" style="background:' + getColor(i, group) + '"></i> ' +
                (type == 'q' ?
                    parseInt(categories[i]) + ' - ' + (i < categories.length - 1 ? parseInt(categories[i + 1] - 1) : max) :
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
 * Method that returns the color regarding to index and type
 * @param {any} index
 * @param {any} type
 */
function getColor(index, type) {
    
    //console.log(index + ' ' + type + ' ' + scales[type][index]);
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
function searchPointData(layer, lat, lon, marker,units) {
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
 */
function plot_layer(idx, layer, time) {
    var wmsLayer = L.tileLayer.wms(geoserver_url, {
        layers: geoserver_workspace + ":" + layer,
        format: 'image/png',
        transparent: true
    }).addTo(maps[idx]);

    wmsLayer.setParams({'time': time});
}

/**
 * Method which updates layers base on user selection
 * */
function update_maps() {
    // Get the values selected in the controls
    var crop = $("#cbo_crop").val();    
    var group = $("#cbo_group").val();
    // Filter dataset original    
    layers_selected = layers_all.filter(function (it) {
        return it.cropID == crop &&
            it.groupID == group ;
    });
    // Realod maps
    load_maps();
}

function update_time() {
    load_maps();
}

/**
 * Method that loads all maps from scratch
 * */
function load_maps() {    
    var time = $("#cbo_time").val();
    var maps_section = '';    
    layers_selected.forEach((value, idx) => { 
        // Condition to validate if start the row 
        if (!(((idx + 1) % 2) == 0))
            maps_section += '<div class="row">';
        // adding a map
        maps_section += '<div class="col-md-6">' +
                            '<h3>' + value.indicator + '</h3>' +
                            '<p class="text-justify">' + value.description + '</p>' +
                            '<div id="map_' + idx + '" class="map_indices"></div>' +
                        '</div>';
        // Condition to validate if end the row 
        if (((idx + 1) % 2) == 0 || (idx == (layers_selected.lenght-1)))
            maps_section +=  '</div>';

    });
    // Adding HTML
    $("#maps_section").html(maps_section);
    // Clear all maps
    maps = [];
    // Loading maps
    layers_selected.forEach((value, idx) => {
        // ploting maps
        plot_map('map_' + idx, idx, value.min, value.max, value.groupID, value.type, value.categories);
        // Adding layer
        plot_layer(idx, value.cropID + "_" + value.indicatorID, time);
    });
    
}


