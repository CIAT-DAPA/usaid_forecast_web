var layerNames;

/**
  * Method that plots a map with the weather stations
  * @param {any} id id div
  * @param {any} ws list of weather stations
  * @param {any} crops list of weather stations with crops
  */
async function plot_map(id, ws, crops) {
    var map = L.map(id).setView([ws[0].latitude, ws[0].longitude], 6);
    L.tileLayer('http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>',
        subdomains: ['a', 'b', 'c']
    }).addTo(map);


    add_map_overlays(map, [
        { "layerName": "ao_adm1", "label": layerNames[0] },
        { "layerName": "ao_adm2", "label": layerNames[1] }
    ]);

    




    for (var i = 0; i < ws.length; ++i) {
        // Search crops with data
        var crops_ws = search_ws_crop(crops, ws[i].id);
        var text = '<table><tr><th>' +
            '<a href="/Clima/' + ws[i].state + '/' + ws[i].municipality + '/' + ws[i].name + '"><span class="glyphicon glyphicon-asterisk"></span> ' + ws[i].state + ', ' + ws[i].municipality + ', ' + ws[i].name + '</a>' +
            '</th></tr>';
        ;
        // Add data crops
        for (var j = 0; j < crops_ws.length; j++) {
            text = text + '<tr><td>' +
                '<a href="/Cultivo/' + crops_ws[j].state + '/' + crops_ws[j].municipality + '/' + crops_ws[j].name + '/' + crops_ws[j].crop + '"><span class="glyphicon glyphicon-grain"></span> ' + crops_ws[j].crop + '</a>' +
                '</td></tr>';
        }
        text = text + '</table>';
        // add mark
        L.marker([ws[i].latitude, ws[i].longitude])
            .bindPopup(text)
            .addTo(map);
    }
}

/**
 * Method that search ws with data about crops
 * @param {any} crops
 * @param {any} ws
 */
function search_ws_crop(crops, ws) {
    var answer = [];
    for (var i = 0; i < crops.length; i++) {
        if (crops[i].id === ws) {
            answer.push(crops[i]);
        }
    }
    return answer;
}
