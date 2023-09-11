

// Global variable for map
var conf;
var geoserver_url;
var geoserver_workspace;
var geoserverLayers = [];
var indicatorYearConstants;
var mapOverlays;
var currentLocale;


async function init() {


    $('#capabilitiesLoader').toggle(true)
    $('#capabilitiesFilter').toggle(false)
    try {
        var capabilities = await get_geoserver_wms_capabilities();

        console.log("wms capabilities", capabilities)
        geoserverLayers = capabilities.WMS_Capabilities.Capability.Layer.Layer;
        updateMetadata(capabilities.WMS_Capabilities.Capability.Layer);
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
async function get_geoserver_wms_capabilities() {

    var res = await axios.get(geoserver_url + "?service=wms&request=GetCapabilities&version=1.3.0&AcceptLanguages=" + currentLocale);
    var x2js = new X2JS();
    return x2js.xml_str2json(res.data);
}

function updateMetadata(mainLayer) {
    $('#indicatorsTitle').html(mainLayer.Title);
    $('#indicatorsDesc').html(mainLayer.Abstract);

}

function updateSelectionEl(layers) {
    // console.log("layers", layers);


    let crops = [];
    let groups = [];

    //sort by order
    layers = layers.sort((a, b) => {
        let orderA = extractKeyword(a, 'order') ?? 1000;
        let orderB = extractKeyword(b, 'order') ?? 1000;
        return orderA - orderB;
    })


    layers.forEach(layer => {
        let crop = extractKeyword(layer, 'crop')
        if (crop) {
            crops.push(crop)
        }
        let group = extractKeyword(layer, 'group')
        if (group) {
            groups.push(group)
        }
    });



    // make unique and remove NA values
    crops = [...new Set(crops.filter(c => c.toLowerCase() != "NA".toLowerCase()))];
    groups = [...new Set(groups.filter(c => c.toLowerCase() != "NA".toLowerCase()))];

    //console.log(crops, groups);

    let cropSelect = $('#cbo_crop')[0];
    $('#cropSelect').toggle(crops.length > 0);
    if (cropSelect) {
        crops.forEach(c =>
            cropSelect.add(new Option(translations[c] ?? c, c))
        )
    }

    let groupSelect = $('#cbo_group')[0];
    $('#groupSelect').toggle(groups.length > 0);
    if (groupSelect) {
        groups.forEach(g =>
            groupSelect.add(new Option(translations[g] ?? g, g))
        )
    }
}





/**
 * Method that loads all maps from scratch
 * */
function initMaps() {

    var crop = $("#cbo_crop").val();
    var group = $("#cbo_group").val();


    //console.log("update_maps", crop, group);
    let filteredLayers = geoserverLayers.filter(layer =>
        (!crop || extractKeyword(layer, 'crop') == crop)
        && (!group || extractKeyword(layer, 'group') == group)
    )


    console.log("filteredLayers", filteredLayers)

    const mapContainer = document.getElementById('maps_section');
    while (mapContainer.firstChild) {
        mapContainer.firstChild.remove()
    }

    //create a map for each layer
    filteredLayers.forEach((layer, idx) => {

        const mapSectionEl = document.createElement('div');
        mapSectionEl.classList.add('indicator-map-container');



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




async function plotMap(mapEl, layer) {



    // create map
    let map = L.map(mapEl, {
        attributionControl: false,
        zoomControl: false,
        timeDimension: true,
        timeDimensionOptions: {

        },
    }).fitBounds(conf.bounds, { padding: [50, 50] });
    map.myData = {};
    map.createPane('left');
    map.createPane('right');

    //add background layer
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>',
        subdomains: ['a', 'b', 'c']
    }).addTo(map);

    L.control.attribution({
        position: 'topright'
    }).addTo(map);

    //add zoom ctrl
    L.control.zoom({ position: 'topright' }).addTo(map);


    //add geoserver layer
    addWMSLayer(map, layer);

    // add legend
    addWMSLegend(map, layer);

    //add compare control
    addCompareCtrl(map, layer)

    //create custom period control
    addPeriodCtrl(map, layer);
    let periods = getPeriods(layer);

    addDownloadCtrl(map, layer);

    let period = periods[0]?.split('-')[0] ?? "01"; // use default if just one period is defined
    map.currentPeriod = period;
    updatePeriod(map, layer)

    let avgDate = new Date(indicatorYearConstants['avg'], period, 1)
    map.timeDimension.setCurrentTime(avgDate)



    // console.log(map)

    //add optional layers
    if (mapOverlays && mapOverlays.length > 0) {
        addGeoJSONOverlays(map, mapOverlays);
        
    }

}




function addPeriodCtrl(map, layer) {
    let periods = getPeriods(layer);
    if (periods.length == 0) return;
    var periodControl = L.control({ position: 'bottomleft' });
    periodControl.onAdd = function (map) {
        var container = L.DomUtil.create('div', 'leaflet-bar leaflet-bar-horizontal leaflet-bar-timecontrol leaflet-control select-control');

        let label = L.DomUtil.create('span');
        label.innerHTML = translations.period + ': ';
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
        return [];
    }
    return periodString.replaceAll('period=[', '').replaceAll(']', '').split(",");

}

function updatePeriod(map, layer) {

    let period = map.currentPeriod;
    let times = layer.Dimension.toString().split(",");

    let dates = times.filter(time => {
        var date = new Date(time);
        return date.getUTCMonth() + 1 == period;// && !Object.values(indicatorYearConstants).includes(date.getUTCFullYear());
    }).map(time => new Date(time));
    map.timeDimension.setAvailableTimes(dates, "replace")

}


function addWMSLayer(map, layer) {



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

            if (date.getUTCFullYear() == indicatorYearConstants['avg'])
                return translations.average;
            else if (date.getUTCFullYear() == indicatorYearConstants['nino'])
                return translations.el_nino;
            else if (date.getUTCFullYear() == indicatorYearConstants['nina'])
                return translations.la_nina;
            else
                return date.getUTCFullYear();
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
        let headingEl = document.createElement("div");
        headingEl.classList.add('legend-heading');
        let acronym = extractKeyword(layer, 'acronym');
        if (acronym) {
            let titleEl = document.createElement("h5");
            titleEl.innerHTML = acronym;
            headingEl.appendChild(titleEl);

        }

        let unit = extractKeyword(layer, 'units');
        if (unit) {
            let unitEl = document.createElement("span");
            unitEl.innerHTML = '(' + unit + ')';
            headingEl.appendChild(unitEl);
        }

        legendDiv.appendChild(headingEl);



        let legendUrl = geoserver_url + "?service=WMS&request=GetLegendGraphic&format=image%2Fpng&layer=" + layer.Name;
        legendUrl += "&WIDTH=10&HEIGHT=10";
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
    var compareControl = L.control({ position: 'bottomright' });
    compareControl.onAdd = function (map) {
        var container = L.DomUtil.create('div', 'leaflet-bar leaflet-bar-horizontal leaflet-bar-timecontrol leaflet-control select-control');

        let label = L.DomUtil.create('span');
        label.innerHTML = translations.compare + ': ';
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
            map.currentCompare = selectedValue;
            updateComparison(map, layer)
        });

        return container;
    }
    compareControl.addTo(map);
}



function updateComparison(map, layer) {
  
    let compare = map.currentCompare ?? 'none';

    if (map.myData.wmsLayerCompare) {
        map.myData.wmsLayerCompare.removeFrom(map)
        map.myData.wmsLayerCompare = null;
    }
    if (map.myData.sideBySideControl) {
        map.myData.sideBySideControl.remove();
        map.myData.sideBySideControl = null;
    }

    if (map.myData.comparisonLabel) {
        map.myData.comparisonLabel.remove();
        map.myData.comparisonLabel = null;
    }

    if (compare.toLowerCase() == 'none') {
        return;
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
    map.myData.sideBySideControl = L.control.sideBySide([tdWmsLayer], [map.myData.wmsLayerCompare]);

    map.myData.sideBySideControl.addTo(map);
    map.invalidateSize()


}




function addDownloadCtrl(map, layer) {


    var downloadControl = L.control({ position: 'topright' });
    downloadControl.onAdd = function (map) {


        var container = L.DomUtil.create('div', 'leaflet-bar leaflet-control');
        var button = L.DomUtil.create('a', 'leaflet-control-button fa fa-download', container);
        button.setAttribute('href', '#');
        button.setAttribute('role', 'button');
        L.DomEvent.disableClickPropagation(button);
        L.DomEvent.on(button, 'click', function () {
            let date = map.timeDimension.getCurrentTime();
            let dateStr = date.getUTCFullYear() + '-' + date.getUTCMonth()

            let url = geoserver_url;
            url += '?service=WCS&request=GetCoverage&version=2.0.1';
            url += '&coverageId=' + layer.Name;
            url += '&format=image/geotiff';
            url += '&time=' + dateStr

            console.log(url)
            const a = document.createElement('a')
            a.href = url
            a.download = url.split('/').pop()
            a.click()

        });
        container.title = "Download";

        return container;
    }
    downloadControl.addTo(map);
}

function extractKeyword(layer, key) {
    let keywords = layer.KeywordList.Keyword;
    if (!Array.isArray(keywords)) {
        console.warn("Keywords are not sufficiently defined", layer)
        return null;
    }

    let valueStr = keywords.find(keyword =>
        (typeof keyword === 'string' || keyword instanceof String) && keyword.startsWith(key)
    )

    let value = valueStr?.split('=')[1];
    //console.log("extractKeyword", keywords, key, value)
    return value;
}