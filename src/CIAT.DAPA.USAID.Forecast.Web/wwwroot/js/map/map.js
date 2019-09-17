/**
  * Method that plots a map with the weather stations
  * @param {any} id id div
  * @param {any} ws list of weather stations
  */
function plot_map(id, ws) {
    var map = L.map('map').setView([ws[0].latitude, ws[0].longitude], 6);
    L.tileLayer('http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>',
        subdomains: ['a', 'b', 'c']
    }).addTo(map);

    for (var i = 0; i < ws.length; ++i) {
        L.marker([ws[i].latitude, ws[i].longitude])
            .bindPopup('<a href="/Clima/' + ws[i].state + '/' + ws[i].municipality + '/' + ws[i].name + '"><span class="glyphicon glyphicon-asterisk"></span> ' + ws[i].state + ', ' + ws[i].municipality + ', ' + ws[i].name + '</a>')
            .addTo(map);
    }
}