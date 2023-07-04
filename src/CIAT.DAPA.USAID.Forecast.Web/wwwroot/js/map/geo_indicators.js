

// Global variable for map
var conf;
var geoserver_url;
var geoserver_workspace;
var geoserverLayers = [];
var indicatorYearConstants;

async function init() {
   
    $('#capabilitiesLoader').toggle(true)
    $('#capabilitiesFilter').toggle(false)
    try {
        var capabilities = await get_geoserver_capabilities();
        geoserverLayers = capabilities.WMS_Capabilities.Capability.Layer.Layer;
        updateSelectionEl(geoserverLayers);
        $('#capabilitiesFilter').toggle(true)
        initMaps();
     
      
    } catch (e) {
        console.error(e);
        $('#errorMsg').html("An error happened. Please try again later");

    }

    $('#capabilitiesLoader').toggle(false)
   

}

// load getcapabilities content from geoserver
async function get_geoserver_capabilities() {
    var res = await axios.get(geoserver_url + "?service=wms&request=GetCapabilities&version=1.3.0");
    var x2js = new X2JS();
    return x2js.xml_str2json(res.data);
}



function updateSelectionEl(layers) {
    //console.log("layers", layers);

   
    let crops = [];
    let groups = [];
  
    layers.forEach(layer => {

        let keywords = layer.KeywordList.Keyword;

        let cropString = keywords.find(k => k.startsWith("crop"))
        let crop = cropString?.replaceAll('crop=', '');
        if (crop) {
            crops.push(crop)
        }

        let groupString = keywords.find(k => k.startsWith("group"))
        let group = groupString?.replaceAll('group=', '');
        if (group) {
            groups.push(group)
        }
    });
    
    // make unique and remove NA values
    crops = [...new Set(crops.filter(c => c.toLowerCase() != "NA".toLowerCase()))];
    groups = [...new Set(groups.filter(c => c.toLowerCase() != "NA".toLowerCase()))];
   
    //console.log(crops, groups);

    let cropSelect = $('#cbo_crop')[0];
    $('#cropSelect').toggle(crops.length>0);
    if (cropSelect) {
        crops.forEach(c =>
            cropSelect.add(new Option(translations[c], c))
        )
    }

    let groupSelect = $('#cbo_group')[0];
    $('#groupSelect').toggle(groups.length > 0);
    if (groupSelect) {
        groups.forEach(g =>
            groupSelect.add(new Option(translations[g], g))
        )
    }
}



function extractKeyword(str) {
    if (typeof str === 'string' || str instanceof String) {
        let parts = str.split('=');
        return {
            key: parts[0],
            val: parts[1]
        }
    } else {
        return {
            key: null,
            val: null
        }
    }
}






/**
 * Method that loads all maps from scratch
 * */
function initMaps() {

    var crop = $("#cbo_crop").val();
    var group = $("#cbo_group").val();
   

   // console.log("update_maps", crop, group);
    let filteredLayers = geoserverLayers.filter(layer => {
        let res = false;
        let keywords = layer.KeywordList.Keyword;

        if (crop) {
            res = keywords.findIndex(k => {
                let keyVal = extractKeyword(k);
                return keyVal.key = 'crop' && keyVal.val == crop;
            })>-1;
        }
     
        if (group) {
            res = keywords.findIndex(k => {
                let keyVal = extractKeyword(k);
                return keyVal.key = 'group' && keyVal.val == group;
            }) > -1;
        }

        return res;
    })



    const mapContainer = document.getElementById('maps_section');
    while (mapContainer.firstChild) {
        mapContainer.firstChild.remove()
    }

    //create a map for each layer
    filteredLayers.forEach((layer, idx) => {

        const mapSectionEl = document.createElement('div');
        mapSectionEl.classList.add('col-md-6');

        const title = document.createElement('h3');
        title.textContent = layer.Title;
        mapSectionEl.appendChild(title);

        const description = document.createElement('p');
        description.textContent = layer.Abstract;
        mapSectionEl.appendChild(description);

        const mapEl = document.createElement('div');
        mapEl.setAttribute('id', 'map_' + idx);
        mapEl.classList.add('map_indices');
        mapSectionEl.appendChild(mapEl);

        const mapContainer = document.getElementById('maps_section');
        mapContainer.appendChild(mapSectionEl);

        plotMap(mapEl, layer)

    });
}


// return the localized name of a month number
var getMonthName = function (idx) {
    var objDate = new Date();
    objDate.setDate(1);
    objDate.setMonth(idx - 1);
    return objDate.toLocaleString("en-us", { month: "short" });
}




function plotMap(mapEl, layer) {


    // create map
    let map = L.map(mapEl, {
        zoomControl: false,
        timeDimension: true,
        timeDimensionOptions: {},
    }).fitBounds(conf.bounds, { padding: [50, 50] });
    map.myData = {};

    //add zoom ctrl
    L.control.zoom({ position: 'topright' }).addTo(map);

    //add background layer
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>',
        subdomains: ['a', 'b', 'c']
    }).addTo(map);

    //add geoserver layer
    addWMSLayer(map, layer);

    // add legend
    addWMSLegend(map, layer);

    //add compare control
    addCompareCtrl(map, layer)

    //create custom period control
    addPeriodCtrl(map, layer);
    let periods = getPeriods(layer);
    let period = periods[0].split('-')[0];
    map.currentPeriod = period;

    updatePeriod(map, layer)

    console.log(map)

}




function addPeriodCtrl(map, layer) {
    let periods = getPeriods(layer);
    var periodControl = L.control({ position: 'bottomleft' });
    periodControl.onAdd = function (map) {
        var container = L.DomUtil.create('div', 'leaflet-bar leaflet-bar-horizontal leaflet-bar-timecontrol leaflet-control select-control');

        let label = L.DomUtil.create('span');
        label.innerHTML = translations.period+': '  ;
        container.appendChild(label);

        let select = L.DomUtil.create('select', 'period-selector');
      periods.forEach(period => {
            let parts = period.split('-')
            let start = parts[0];
            let label = "";
            parts.forEach(part => {
                label += getMonthName(part) + " - ";
            })
            label = label.slice(0, -3);
            select.add(new Option(label, start))
        })
        container.appendChild(select);


        // Add event listener to the select element
        select.addEventListener('change', function (event) {
            var selectedValue = event.target.value;
            map.currentPeriod = selectedValue;

            updatePeriod(map, layer)
            updateComparison(map, layer)

        });

        return container;
    }
    periodControl.addTo(map);
}

function getPeriods(layer) {
    let keywords = layer.KeywordList.Keyword;

    let periodString = keywords.find(k => k.startsWith("period"))
    if (!periodString) {
        return ['01'];
    }
    return periodString.replaceAll('period=[', '').replaceAll(']', '').split(",");

}

function updatePeriod(map, layer) {

    let period = map.currentPeriod;
    let times = layer.Dimension.toString().split(",");
    let dates = times.filter(time => {
        var date = new Date(time);
        return date.getUTCMonth() + 1 == period && !Object.values(indicatorYearConstants).includes(date.getUTCFullYear());
    })
    map.timeDimension.setAvailableTimes(dates, "replace")
}


function addWMSLayer(map, layer) {


    console.log('addWMSLayer')
    // plot indicator layer
    let wmsLayer = L.tileLayer.wms(geoserver_url, {
        layers: geoserver_workspace + ":" + layer.Name,
        format: 'image/png',
        transparent: true,
       // opacity: 0.9,
    })

    // Create and add a TimeDimension Layer to the map
    map.myData.tdWmsLayer = L.timeDimension.layer.wms(wmsLayer);
    map.myData.tdWmsLayer.addTo(map);

    L.Control.TimeDimensionCustom = L.Control.TimeDimension.extend({
        _getDisplayDateFormat: function (date) {
            return date.getFullYear();
        }
    });
    var timeDimensionControl = new L.Control.TimeDimensionCustom({
        speedSlider: false,
        playerOptions: {
            buffer: 1,
            minBufferReady: -1,
        }
    });
    map.addControl(timeDimensionControl);
}

function addWMSLegend(map, layer) {
    // add legend
    var legendControl = L.control({ position: 'topleft' });
    legendControl.onAdd = function (map) {
        var legendDiv = L.DomUtil.create('div', 'legend');
        let legendUrl = geoserver_url + "?service=WMS&request=GetLegendGraphic&format=image%2Fpng&layer=" + layer.Name;
        legendUrl += "&WIDTH=10&HEIGHT=20";
        legendUrl += "&LEGEND_OPTIONS=";
        legendUrl += "fontStyle:bold;";
        legendUrl += "fontAntiAliasing:true;";
        legendUrl += "dx:10.2;dy:0.2;mx:0;my:0;";
        legendUrl += "forceLabels:on;";
        legendUrl += "forceTitles:on;";

        let img = document.createElement("img");
        img.src = legendUrl;
        legendDiv.appendChild(img);

        return legendDiv;
    };

    legendControl.addTo(map);
}







function addCompareCtrl(map, layer) {
    var compareControl = L.control({ position: 'bottomleft' });
    compareControl.onAdd = function (map) {
        var container = L.DomUtil.create('div', 'leaflet-bar leaflet-bar-horizontal leaflet-bar-timecontrol leaflet-control select-control');

        let label = L.DomUtil.create('span');
        label.innerHTML = translations.compare+': ';
        container.appendChild(label);

        let select = L.DomUtil.create('select', 'period-selector');
        select.add(new Option(translations.none, "none"))
        select.add(new Option(translations.average, indicatorYearConstants["avg"]))
        select.add(new Option(translations.el_nino, indicatorYearConstants["nino"]))
        select.add(new Option(translations.la_nina, indicatorYearConstants["nina"]))

        container.appendChild(select);

        // Add event listener to the select element
        select.addEventListener('change', function (event) {
            var selectedValue = event.target.value;
           // console.log('Selected value:', selectedValue);
            map.currentCompare = selectedValue;
            updateComparison(map, layer)
        });

        return container;
    }
    compareControl.addTo(map);
}



function updateComparison(map, layer) {

    let compare = map.currentCompare ?? 'none';
    if (compare.toLowerCase() == 'none') {
        if (map.myData.wmsLayerCompare) {
            wmsLayerCompare.removeFrom(map)
            map.myData.wmsLayerCompare = null;
        }
        if (map.myData.sideBySideControl) {
            map.myData.sideBySideControl.remove();
            map.myData.sideBySideControl = null;
        }
        return;
    }



    if (map.myData.wmsLayerCompare) {
        map.myData.wmsLayerCompare.removeFrom(map)
        map.myData.wmsLayerCompare = null;
    }

    map.myData.wmsLayerCompare = L.tileLayer.wms(geoserver_url, {
        layers: geoserver_workspace + ":" + layer.Name,
        format: 'image/png',
        transparent: true,
        zIndex: 100,
       // opacity: 0.9,
    })
    map.myData.wmsLayerCompare.addTo(map);
    map.myData.wmsLayerCompare.setParams({ 'time': compare + "-" + map.currentPeriod });




    // hack around bug with getContainer, that is undefined for wmsLayer
    let tdWmsLayer = map.myData.tdWmsLayer;
    tdWmsLayer.getContainer = tdWmsLayer.getPane;

    map.createPane("left");
    map.createPane("right");


    if (map.myData.sideBySideControl) {
        map.myData.sideBySideControl.remove();
        map.myData.sideBySideControl = null;
    }
    map.myData.sideBySideControl = L.control.sideBySide(tdWmsLayer, map.myData.wmsLayerCompare);
    map.myData.sideBySideControl.addTo(map);

    
    /*
    // Add legends
    var legend2 = L.control({ position: 'topright' });

    legend2.onAdd = function (map) {
        var div = L.DomUtil.create('div', 'info legend');
        div.innerHTML = '<strong>Compare</strong>';
        return div;
    };
    legend2.addTo(map);

   var legend3 = L.control({ position: 'topleft' });

    legend3.onAdd = function (map) {
        var div = L.DomUtil.create('div', 'info legend');
        div.innerHTML = '<strong>' + $("#cbo_time option:selected").text() + '</strong>';
        return div;
    };
    legend3.addTo(map);
    */
}

